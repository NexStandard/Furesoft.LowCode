﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Furesoft.LowCode.Designer.Core.NodeBuilding;
using Furesoft.LowCode.Designer.ViewModels;
using Furesoft.LowCode.Editor.Model;
using NiL.JS.Core;
using NiL.JS.Extensions;

namespace Furesoft.LowCode.Designer.Core;

[DataContract(IsReference = true)]
public abstract class VisualNode : ViewModelBase, ICustomTypeDescriptor
{
    private string _label;
    private string _description;
    internal Evaluator _evaluator;
    protected Context Context { get; private set; }

    public VisualNode(string label)
    {
        Label = label;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Browsable(false)]
    public string Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Browsable(false)]
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// Gets the previously executed node
    /// </summary>
    [Browsable(false)]
    public VisualNode PreviousNode { get; set; }

    public abstract Task Execute();

    protected async Task ContinueWith(IOutputPin pin, Context context = null,
        [CallerArgumentExpression("pin")] string pinMembername = null)
    {
        _evaluator.Debugger.ResetWait();

        var pinName = GetPinName(pinMembername);
        var connections = GetConnections();
        var pinViewModel = GetPinViewModel(pinName);

        var pinConnections = GetPinConnections(connections, pinViewModel);

        foreach (var pinConnection in pinConnections)
        {
            InitNextNode(pinConnection, pinName, out var parent, context);

            if (_evaluator.Debugger.IsAttached)
            {
                await _evaluator.Debugger.WaitTask;
            }

            await parent?.Execute();
        }
    }

    private void InitNextNode(IConnector pinConnection, string pinName, out VisualNode parentNode,
        Context context = null)
    {
        CustomNodeViewModel parent;

        if (pinConnection.Start.Name == pinName)
        {
            parent = pinConnection.End.Parent as CustomNodeViewModel;
        }
        else if (pinConnection.End.Name == pinName)
        {
            parent = pinConnection.Start.Parent as CustomNodeViewModel;
        }
        else
        {
            parentNode = null;
            return;
        }

        parent.DefiningNode._evaluator = _evaluator;
        parent.DefiningNode.Context = context ?? _evaluator.Context;
        parent.DefiningNode.Drawing = Drawing;
        parent.DefiningNode.PreviousNode = this;
        parent.DefiningNode._evaluator.Debugger.CurrentNode = parent.DefiningNode;

        parentNode = parent.DefiningNode;
    }

    [Browsable(false)]
    public bool HasBreakPoint
    {
        get
        {
            return _evaluator?.Debugger.BreakPointNodes.Contains(this) ?? false;
        }
    }

    public void AddBreakPoint()
    {
        _evaluator.Debugger.BreakPointNodes.Add(this);
    }

    public void RemoveBreakPoint()
    {
        _evaluator.Debugger.BreakPointNodes.Remove(this);
    }

    private static IEnumerable<IConnector> GetPinConnections(IEnumerable<IConnector> connections, IPin pinViewModel)
    {
        return from conn in connections
            where conn.Start == pinViewModel || conn.End == pinViewModel
            select conn;
    }

    private IPin GetPinViewModel(string pinName)
    {
        return (from node in Drawing.Nodes
            where ((CustomNodeViewModel)node).DefiningNode == this
            from pinn in node.Pins
            where pinn.Name == pinName
            select pinn).FirstOrDefault();
    }

    private IEnumerable<IConnector> GetConnections()
    {
        return from connection in Drawing.Connectors
            where ((CustomNodeViewModel)connection.Start.Parent).DefiningNode == this
                  || ((CustomNodeViewModel)connection.End.Parent).DefiningNode == this
            select connection;
    }

    private string GetPinName(string propertyName)
    {
        var propInfo = GetType().GetProperty(propertyName);

        var attr = propInfo.GetCustomAttribute<PinAttribute>();

        if (attr == null)
        {
            return propInfo.Name;
        }

        return attr.Name;
    }

    protected T Evaluate<T>(string src)
    {
        return Context.Eval(src).As<T>();
    }

    protected void SetOutVariable(string name, object value)
    {
        Context.GetVariable(name).Assign(JSValue.Wrap(value));
    }
    
    protected void DefineConstant(string name, object value, Context context = null)
    {
       (context ?? Context).DefineVariable(name).Assign(JSValue.Wrap(value));
    }

    public string GetCallStack()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{Label}:");
        foreach (PropertyDescriptor value in GetProperties())
        {
            sb.AppendLine($"\t{value.Name}: {value.GetValue(this)}");
        }

        sb.AppendLine(PreviousNode?.GetCallStack());

        return sb.ToString();
    }

    #region Custom Type Descriptor Interfaces

    public AttributeCollection GetAttributes()
    {
        return TypeDescriptor.GetAttributes(this, true);
    }

    public string GetClassName()
    {
        return TypeDescriptor.GetClassName(this, true);
    }

    public string GetComponentName()
    {
        return TypeDescriptor.GetComponentName(this, true);
    }

    public TypeConverter GetConverter()
    {
        return TypeDescriptor.GetConverter(this, true);
    }

    public EventDescriptor GetDefaultEvent()
    {
        return TypeDescriptor.GetDefaultEvent(this, true);
    }

    public PropertyDescriptor GetDefaultProperty()
    {
        return TypeDescriptor.GetDefaultProperty(this, true);
    }

    public object GetEditor(Type editorBaseType)
    {
        return TypeDescriptor.GetEditor(this, editorBaseType, true);
    }

    public EventDescriptorCollection GetEvents()
    {
        return TypeDescriptor.GetEvents(this, true);
    }

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
        return TypeDescriptor.GetEvents(this, attributes, true);
    }

    public PropertyDescriptorCollection GetProperties()
    {
        return GetProperties(null);
    }

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
        return new(
            (from PropertyDescriptor property in TypeDescriptor.GetProperties(this, true)
                where property.PropertyType != typeof(IInputPin) && property.PropertyType != typeof(IOutputPin)
                let attribute = property.Attributes.OfType<BrowsableAttribute>().FirstOrDefault()
                where attribute == null || attribute.Browsable
                select TypeDescriptor.CreateProperty(GetType(), property.Name, property.PropertyType)).ToArray());
    }

    public object GetPropertyOwner(PropertyDescriptor pd)
    {
        return this;
    }

    #endregion
}

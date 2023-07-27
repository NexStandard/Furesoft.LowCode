﻿using System;
using System.ComponentModel;
using System.Linq;
using Furesoft.LowCode.Designer.Core.NodeBuilding;

namespace Furesoft.LowCode.Designer.Core;

public partial class VisualNode
{
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

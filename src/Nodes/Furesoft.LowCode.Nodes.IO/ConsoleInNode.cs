﻿using System.ComponentModel;
using System.Runtime.Serialization;
using Furesoft.LowCode.Core;
using Furesoft.LowCode.Core.Components.Views;
using Furesoft.LowCode.Core.NodeBuilding;
using NodeEditor.Model;

namespace Furesoft.LowCode.Nodes.IO;

[Description("Read a value from the console")]
[NodeCategory("IO")]
[NodeView(typeof(IconNodeView), "M67.5375 87.1125 5.6625 148.9875A12.5 12.5 0 1023.3375 166.6625L94.05 95.95A12.5 12.5 0 0094.05 78.275L23.3375 7.5625A12.5 12.5 0 005.6625 25.2375L67.5375 87.1125zM87 173 190 172A12.5 12.5 0 00185 143L93 144A12.5 12.5 0 0086 173z")]
public class ConsoleInNode : VisualNode
{
    [Description("The input from the console")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Output { get; set; }

    [Pin("Flow Input", PinAlignment.Top)]
    public IInputPin InputPin { get; set; }

    [Pin("Flow Output", PinAlignment.Bottom)]
    public IOutputPin OutputPin { get; set; }
    
    public ConsoleInNode() : base("Console Input")
    {
    }

    public override Task Execute()
    {
        var input = Console.ReadLine();
        SetOutVariable(Output, input);

        return ContinueWith(OutputPin);
    }
}
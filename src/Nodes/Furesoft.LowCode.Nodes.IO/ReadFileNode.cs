﻿using System.ComponentModel;
using System.Runtime.Serialization;
using Furesoft.LowCode.Designer.Core;
using Furesoft.LowCode.Designer.Core.Components.Views;
using Furesoft.LowCode.Designer.Core.NodeBuilding;
using Furesoft.LowCode.Editor.Model;

namespace Furesoft.LowCode.Nodes.IO;

[Description("Read a file")]
[NodeCategory("IO/Files")]
[NodeView(typeof(IconNodeView), "M141.2438 300H42A37.5 37.5 0 014.5 262.5V37.5A37.5 37.5 0 0142 0H192A37.5 37.5 0 01229.5 37.5V211.7438A18.75 18.75 0 01224.0063 225L154.5 294.5063A18.75 18.75 0 01141.2438 300zM145.125 234.375V271.875L201.375 215.625H163.875A18.75 18.75 0 00145.125 234.375zM126.375 159.375V88.2562L147.8625 109.7625A9.375 9.375 0 00161.1375 96.4875L123.6375 58.9875A9.375 9.375 0 00110.3625 58.9875L72.8625 96.4875A9.375 9.375 0 0086.1375 109.7625L107.625 88.2562V159.375A9.375 9.375 0 00126.375 159.375z")]
public class ReadFileNode : VisualNode
{
    [Description("Destination Filename")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Filename { get; set; }
    
    [Description("Where to store the file content")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Output { get; set; }

    [Pin("Flow Input", PinAlignment.Top)]
    public IInputPin InputPin { get; set; }

    [Pin("Flow Output", PinAlignment.Bottom)]
    public IOutputPin OutputPin { get; set; }
    
    public ReadFileNode() : base("Read File")
    {
    }

    public override Task Execute(CancellationToken cancellationToken)
    {
        var filename = Evaluate<string>(Filename);
       
        SetOutVariable(Output, File.ReadAllText(filename));

        return ContinueWith(OutputPin, cancellationToken: cancellationToken);
    }
}

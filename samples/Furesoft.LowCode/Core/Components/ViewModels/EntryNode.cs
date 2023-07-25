﻿using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Furesoft.LowCode.Core.Components.Views;
using Furesoft.LowCode.Core.NodeBuilding;
using NodeEditor.Model;

namespace Furesoft.LowCode.Core.Components.ViewModels;

[DataContract(IsReference = true)]
[IgnoreTemplate]
[NodeView(typeof(EntryView))]
[Description("The starting node of the graph")]
public class EntryNode : VisualNode
{
    public EntryNode() : base("Entry")
    {
    }
    
    [Pin("Flow Output", PinAlignment.Bottom)]
    public IOutputPin FlowPin { get; set; }

    public override Task Execute()
    {
        return ContinueWith(FlowPin);
    }
}

﻿using System.ComponentModel;
using Furesoft.LowCode.Designer.Core;
using Timer = System.Timers.Timer;

namespace Furesoft.LowCode.Nodes.Scheduling;

[NodeCategory("Scheduling")]
[Description("Continue ellapsed every interval")]
public class TimerNode : InputOutputNode
{
    public TimerNode() : base("Timer")
    {
    }

    [Pin("Ellapsed", PinAlignment.Right)] 
    public IOutputPin EllapsedPin { get; set; }

    public double Interval { get; set; }

    public override async Task Execute(CancellationToken cancellationToken)
    {
        var timer = new Timer();
        timer.Interval = Interval;
        timer.Elapsed += async (sender, args) =>
        {
            await ContinueWith(EllapsedPin, cancellationToken: cancellationToken);
        };
        timer.Start();

        await ContinueWith(OutputPin, cancellationToken: cancellationToken);
    }
}
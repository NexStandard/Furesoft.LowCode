﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Furesoft.LowCode.Core.Components.Views;

public partial class ConditionView : UserControl
{
    public ConditionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace DockMvvmSample;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var mainWindowViewModel = new MainWindowViewModel();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktopLifetime:
                {
                    var mainWindow = new MainWindow {DataContext = mainWindowViewModel};

                    mainWindow.Closing += (_, _) =>
                    {
                        mainWindowViewModel.CloseLayout();
                    };

                    desktopLifetime.MainWindow = mainWindow;

                    desktopLifetime.Exit += (_, _) =>
                    {
                        mainWindowViewModel.CloseLayout();
                    };

                    break;
                }
            case ISingleViewApplicationLifetime singleViewLifetime:
                {
                    var mainView = new MainView {DataContext = mainWindowViewModel};

                    singleViewLifetime.MainView = mainView;

                    break;
                }
        }

        base.OnFrameworkInitializationCompleted();
    }
}

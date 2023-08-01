﻿using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Furesoft.LowCode.Editor.Behaviors;

public class ConnectorsSelectedBehavior : Behavior<ItemsControl>
{
    private IDisposable? _dataContextDisposable;
    private IDrawingNode? _drawingNode;

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is not null)
        {
            _dataContextDisposable = AssociatedObject
                .GetObservable(StyledElement.DataContextProperty)
                .Subscribe(new AnonymousObserver<object?>(
                    x =>
                    {
                        if (x is IDrawingNode drawingNode)
                        {
                            if (_drawingNode == drawingNode)
                            {
                                _drawingNode.SelectionChanged -= DrawingNode_SelectionChanged;
                            }

                            RemoveSelectedPseudoClasses(AssociatedObject);

                            _drawingNode = drawingNode;

                            _drawingNode.SelectionChanged += DrawingNode_SelectionChanged;
                        }
                        else
                        {
                            RemoveSelectedPseudoClasses(AssociatedObject);
                        }
                    }));
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject is not null)
        {
            if (_drawingNode is not null)
            {
                _drawingNode.SelectionChanged -= DrawingNode_SelectionChanged;
            }

            _dataContextDisposable?.Dispose();
        }
    }

    private void DrawingNode_SelectionChanged(object? sender, EventArgs e)
    {
        if (AssociatedObject?.DataContext is not IDrawingNode)
        {
            return;
        }

        if (_drawingNode is not null)
        {
            var selectedNodes = _drawingNode.GetSelectedNodes();
            var selectedConnectors = _drawingNode.GetSelectedConnectors();

            if (selectedNodes is {Count: > 0} || selectedConnectors is {Count: > 0})
            {
                AddSelectedPseudoClasses(AssociatedObject);
            }
            else
            {
                RemoveSelectedPseudoClasses(AssociatedObject);
            }
        }
    }

    private void AddSelectedPseudoClasses(ItemsControl itemsControl)
    {
        foreach (var control in itemsControl.GetRealizedContainers())
        {
            if (control is not {DataContext: IConnector connector} containerControl)
            {
                continue;
            }

            var selectedConnectors = _drawingNode?.GetSelectedConnectors();

            if (_drawingNode is not null && selectedConnectors is not null && selectedConnectors.Contains(connector))
            {
                if (containerControl is ContentPresenter {Child.Classes: IPseudoClasses pseudoClasses})
                {
                    pseudoClasses.Add(":selected");
                }
            }
            else
            {
                if (containerControl is ContentPresenter {Child.Classes: IPseudoClasses pseudoClasses})
                {
                    pseudoClasses.Remove(":selected");
                }
            }
        }
    }

    private static void RemoveSelectedPseudoClasses(ItemsControl itemsControl)
    {
        foreach (var control in itemsControl.GetRealizedContainers())
        {
            if (control is not {DataContext: IConnector} containerControl)
            {
                continue;
            }

            if (containerControl is ContentPresenter {Child: {Classes: IPseudoClasses pseudoClasses}})
            {
                pseudoClasses.Remove(":selected");
            }
        }
    }
}

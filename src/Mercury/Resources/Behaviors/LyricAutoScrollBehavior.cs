using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Mercury.ViewModels;
using System;
using System.Runtime.CompilerServices;
using Avalonia.Input;

namespace Mercury.Resources.Behaviors;

public class LyricAutoScrollBehavior : AvaloniaObject
{
    private static DispatcherTimer? _activeScrollTimer;
    
    
    public static readonly AttachedProperty<int> CurrentLineIndexProperty =
        AvaloniaProperty.RegisterAttached<LyricAutoScrollBehavior, ScrollViewer, int>(
            "CurrentLineIndex", defaultValue: -1);

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<LyricAutoScrollBehavior, ScrollViewer, bool>(
            "IsEnabled", defaultValue: true);

    // The ItemsControl must be stored so we can find item containers
    public static readonly AttachedProperty<ItemsControl?> ItemsControlProperty =
        AvaloniaProperty.RegisterAttached<LyricAutoScrollBehavior, ScrollViewer, ItemsControl?>(
            "ItemsControl");

    static LyricAutoScrollBehavior()
    {
        CurrentLineIndexProperty.Changed.AddClassHandler<ScrollViewer>(OnCurrentLineIndexChanged);
        IsEnabledProperty.Changed.AddClassHandler<ScrollViewer>(OnIsEnabledChanged);
    }

    public static int  GetCurrentLineIndex(ScrollViewer sv) => sv.GetValue(CurrentLineIndexProperty);
    public static void SetCurrentLineIndex(ScrollViewer sv, int value) => sv.SetValue(CurrentLineIndexProperty, value);

    public static bool GetIsEnabled(ScrollViewer sv) => sv.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(ScrollViewer sv, bool value) => sv.SetValue(IsEnabledProperty, value);

    public static ItemsControl? GetItemsControl(ScrollViewer sv) => sv.GetValue(ItemsControlProperty);
    public static void SetItemsControl(ScrollViewer sv, ItemsControl? value) => sv.SetValue(ItemsControlProperty, value);

    private static void OnCurrentLineIndexChanged(ScrollViewer sv, AvaloniaPropertyChangedEventArgs e)
    {
        if (!GetIsEnabled(sv)) return;
        if (e.NewValue is not int index || index < 0) return;

        var itemsControl = GetItemsControl(sv);
        if (itemsControl is null) return;

        // Use a small delay so the layout has settled after state changes
        DispatcherTimer.RunOnce(() =>
        {
            var container = itemsControl.ContainerFromIndex(index);
            if (container is null) return;

            // Get the item's position relative to the ScrollViewer content
            var transform = container.TransformToVisual(sv);
            if (transform is null) return;

            var itemPos    = transform.Value.Transform(new Point(0, 0));
            var itemCenter = itemPos.Y + container.Bounds.Height / 2;
            var viewCenter = sv.Bounds.Height / 2;

            var targetOffset = sv.Offset.Y + itemCenter - viewCenter;
            targetOffset = Math.Clamp(targetOffset, 0, sv.ScrollBarMaximum.Y);

            SmoothScrollTo(sv, targetOffset);

        }, TimeSpan.FromMilliseconds(50));
    }

    private static void OnIsEnabledChanged(ScrollViewer sv, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            sv.AddHandler(
                InputElement.PointerWheelChangedEvent,
                OnPointerWheelChanged,
                Avalonia.Interactivity.RoutingStrategies.Tunnel,
                handledEventsToo: true);
        }
        else
        {
            sv.RemoveHandler(
                InputElement.PointerWheelChangedEvent,
                OnPointerWheelChanged);
        }
    }

    private static void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is not ScrollViewer sv) return;
        
        _activeScrollTimer?.Stop();
        _activeScrollTimer = null;
        
        sv.SetCurrentValue(IsEnabledProperty, false);
    }

    private static void SmoothScrollTo(ScrollViewer sv, double targetY)
    {
        _activeScrollTimer?.Stop();
        _activeScrollTimer = null;
        
        const double duration = 450; // ms
        const int steps = 40;
        double startY = sv.Offset.Y;
        double distance = targetY - startY;
        int step = 0;

        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(duration / steps)
        };
        
        _activeScrollTimer = timer;
        
        timer.Tick += (_, _) =>
        {
            step++;
            double t         = step / (double)steps;
            double eased     = EaseOutExpo(t);
            sv.Offset        = new Avalonia.Vector(sv.Offset.X, startY + distance * eased);

            if (step >= steps)
            {
                sv.Offset = new Vector(sv.Offset.X, targetY);
                timer.Stop();
                _activeScrollTimer = null;
            }
        };

        timer.Start();
    }

    // Smooth ease curve
    private static double EaseOutExpo(double t)
        => t >= 1 ? 1 : 1 - Math.Pow(2, -10 * t);
}
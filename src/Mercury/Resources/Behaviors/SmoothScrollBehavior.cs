using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace Mercury.Resources.Behaviors;

public class SmoothScrollBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<SmoothScrollBehavior, ScrollViewer, bool>(
            "IsEnabled", defaultValue: false);

    public static readonly AttachedProperty<double> SpeedFactorProperty =
        AvaloniaProperty.RegisterAttached<SmoothScrollBehavior, ScrollViewer, double>(
            "SpeedFactor", defaultValue: 1.0);

    private static readonly ConditionalWeakTable<ScrollViewer, ScrollState> _states = new();

    static SmoothScrollBehavior()
    {
        IsEnabledProperty.Changed.AddClassHandler<ScrollViewer>(OnIsEnabledChanged);
    }

    public static bool GetIsEnabled(ScrollViewer sv) => sv.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(ScrollViewer sv, bool value) => sv.SetValue(IsEnabledProperty, value);

    public static double GetSpeedFactor(ScrollViewer sv) => sv.GetValue(SpeedFactorProperty);
    public static void SetSpeedFactor(ScrollViewer sv, double value) => sv.SetValue(SpeedFactorProperty, value);

    private static void OnIsEnabledChanged(ScrollViewer sv, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            sv.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged,
                Avalonia.Interactivity.RoutingStrategies.Tunnel);
        }
        else
        {
            sv.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);

            if (_states.TryGetValue(sv, out var state))
            {
                state.Timer?.Stop();
                _states.Remove(sv);
            }
        }
    }

    private static ScrollState GetOrCreateState(ScrollViewer sv)
    {
        return _states.GetOrCreateValue(sv);
    }

    private static void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is not ScrollViewer sv) return;

        e.Handled = true;

        var state = GetOrCreateState(sv);
        double speed = GetSpeedFactor(sv);
        double deltaY = -e.Delta.Y * 80 * speed;
        double deltaX = -e.Delta.X * 80 * speed;

        if (!state.IsAnimating)
        {
            state.TargetOffsetY = sv.Offset.Y;
            state.TargetOffsetX = sv.Offset.X;
        }

        state.TargetOffsetY = Math.Clamp(state.TargetOffsetY + deltaY, 0, sv.ScrollBarMaximum.Y);
        state.TargetOffsetX = Math.Clamp(state.TargetOffsetX + deltaX, 0, sv.ScrollBarMaximum.X);

        AnimateScroll(sv, state);
    }

    private static void AnimateScroll(ScrollViewer sv, ScrollState state)
    {
        if (state.IsAnimating) return;

        state.IsAnimating = true;
        const double lerp = 0.15;

        var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Render, (_, _) =>
        {
            double currentY = sv.Offset.Y;
            double currentX = sv.Offset.X;
            double diffY = state.TargetOffsetY - currentY;
            double diffX = state.TargetOffsetX - currentX;

            bool doneY = Math.Abs(diffY) < 0.5;
            bool doneX = Math.Abs(diffX) < 0.5;

            double nextY = doneY ? state.TargetOffsetY : currentY + diffY * lerp;
            double nextX = doneX ? state.TargetOffsetX : currentX + diffX * lerp;

            sv.Offset = new Vector(nextX, nextY);

            if (doneY && doneX)
            {
                state.Timer?.Stop();
                state.Timer = null;
                state.IsAnimating = false;
            }
        });

        state.Timer = timer;
        timer.Start();
    }

    private class ScrollState
    {
        public double TargetOffsetY { get; set; }
        public double TargetOffsetX { get; set; }
        public bool IsAnimating { get; set; }
        public DispatcherTimer? Timer { get; set; }
    }
}

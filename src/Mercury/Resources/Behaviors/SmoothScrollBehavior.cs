using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace Mercury.Resources.Behaviors;

public class SmoothScrollBehavior : AvaloniaObject
{
    // ===== TUNING PARAMETERS =====
    private const double Friction = 0.85;              // Higher = stops faster (0.0 - 1.0)
    private const double MinVelocity = 0.3;           // Velocity threshold to stop animation
    private const double VelocityMultiplier = 20.0;   // Scroll speed per wheel tick
    private const int TimerIntervalMs = 8;           // Animation frame interval (lower = smoother, more CPU)
    // =============================

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
            
            sv.DetachedFromVisualTree += OnDetached;
        }
        else
        {
            sv.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);
            sv.DetachedFromVisualTree -= OnDetached;

            CleanupState(sv);
        }
    }

    private static void OnDetached(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        if (sender is ScrollViewer sv)
        {
            CleanupState(sv);
        }
    }

    private static void CleanupState(ScrollViewer sv)
    {
        if (_states.TryGetValue(sv, out var state))
        {
            state.Timer?.Stop();
            state.Timer = null;
            state.VelocityX = 0;
            state.VelocityY = 0;
        }
    }

    private static ScrollState GetOrCreateState(ScrollViewer sv)
    {
        return _states.GetOrCreateValue(sv);
    }

    private static void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is not ScrollViewer sv) return;

        bool isVertical = e.Delta.Y != 0;
        bool isHorizontal = e.Delta.X != 0;

        bool shiftHeld = (e.KeyModifiers & KeyModifiers.Shift) != 0;
        if (shiftHeld && isVertical)
        {
            isHorizontal = true;
            isVertical = false;
        }

        bool canScrollVertically = sv.ScrollBarMaximum.Y > 0;
        bool canScrollHorizontally = sv.ScrollBarMaximum.X > 0;

        if (isVertical && !canScrollVertically) return;
        if (isHorizontal && !canScrollHorizontally) return;

        e.Handled = true;

        var state = GetOrCreateState(sv);
        double speed = GetSpeedFactor(sv);

        // Add velocity (momentum-based scrolling)
        if (isHorizontal || shiftHeld)
        {
            double delta = shiftHeld ? e.Delta.Y : e.Delta.X;
            state.VelocityX += -delta * VelocityMultiplier * speed;
        }

        if (isVertical && !shiftHeld)
        {
            state.VelocityY += -e.Delta.Y * VelocityMultiplier * speed;
        }

        StartInertiaScroll(sv, state);
    }

    private static void StartInertiaScroll(ScrollViewer sv, ScrollState state)
    {
        if (state.Timer != null) return; // Already running

        var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(TimerIntervalMs), DispatcherPriority.Render, (_, _) =>
        {
            // Apply velocity
            double newX = sv.Offset.X + state.VelocityX;
            double newY = sv.Offset.Y + state.VelocityY;

            // Clamp to valid scroll range
            newX = Math.Clamp(newX, 0, sv.ScrollBarMaximum.X);
            newY = Math.Clamp(newY, 0, sv.ScrollBarMaximum.Y);

            sv.Offset = new Vector(newX, newY);

            // Apply friction
            state.VelocityX *= Friction;
            state.VelocityY *= Friction;

            // Stop when velocity is negligible
            if (Math.Abs(state.VelocityX) < MinVelocity && Math.Abs(state.VelocityY) < MinVelocity)
            {
                state.VelocityX = 0;
                state.VelocityY = 0;
                state.Timer?.Stop();
                state.Timer = null;
            }
        });

        state.Timer = timer;
        timer.Start();
    }

    private class ScrollState
    {
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public DispatcherTimer? Timer { get; set; }
    }
}

using Avalonia.Controls;
using Avalonia.Input;
using Mercury.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Views;

public partial class LyricsViewer : UserControl
{
    public LyricsViewer()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<LyricsViewerViewModel>();

        LyricsScroller.AddHandler(
            InputElement.PointerWheelChangedEvent,
            OnLyricsScrollerWheelChanged,
            Avalonia.Interactivity.RoutingStrategies.Tunnel,
            handledEventsToo: true);
    }

    private void OnLyricsScrollerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (DataContext is LyricsViewerViewModel vm)
            vm.SetAutoScrollCommand.Execute(false);
    }
}
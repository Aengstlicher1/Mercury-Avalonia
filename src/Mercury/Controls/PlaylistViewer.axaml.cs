using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mercury.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Controls;

public partial class PlaylistViewer : UserControl
{
    public PlaylistViewer()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<PlaylistViewerViewModel>();
    }
}
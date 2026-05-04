using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mercury.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Controls;

public partial class QueueViewer : UserControl
{
    public QueueViewer()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<QueueViewerViewModel>();
    }
}
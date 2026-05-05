using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Controls;

public partial class EntityViewer : UserControl
{
    public EntityViewer()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.EntityViewerViewModel>();
    }
}
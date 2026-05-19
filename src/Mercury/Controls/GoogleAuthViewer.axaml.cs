using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mercury.ViewModels;

namespace Mercury.Controls;

public partial class GoogleAuthViewer : UserControl
{
    public GoogleAuthViewer()
    {
        InitializeComponent();
        DataContext = new GoogleAuthViewerViewModel();
    }
}
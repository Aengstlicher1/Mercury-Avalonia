using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Input;
using Mercury.Services;
using Mercury.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel vm, INavigationService ns)
    {
        InitializeComponent();
        DataContext = vm;
        
        Navigator.Content = ns.PageInfos[0].Page;
        ns.Navigation = Navigator;
    }
}
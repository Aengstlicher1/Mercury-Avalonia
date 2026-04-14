using System.Threading.Tasks;
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
    INavigationService _ns;
    SearchPage _sp;
    
    public MainWindow(MainWindowViewModel vm, INavigationService ns, SearchPage sp)
    {
        InitializeComponent();
        DataContext = vm;
        
        _ns = ns;
        Navigator.Content = _ns.PageInfos[0].Page;
        _ns.Navigation = Navigator;

        _sp = sp;
    }

    private void SearchBox_GotFocus(object? sender, FocusChangedEventArgs e)
    {
        _ = _ns.NavigateTo(_sp);
    }
}
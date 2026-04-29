using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Input;
using Avalonia.Interactivity;
using Mercury.Services;
using Mercury.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Views;

public partial class MainWindow : Window
{
    private readonly INavigationService _ns;
    private readonly IPlayerService _ps;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainWindowViewModel>();
        _ns = App.Services.GetRequiredService<INavigationService>();
        _ps = App.Services.GetRequiredService<IPlayerService>();
        
        _ns.SetHost(Navigator);
        Closing += (s, e) =>
        {
            _ps.Dispose();
        };
    }

    private void SearchBox_GotFocus(object? sender, FocusChangedEventArgs e)
    {
        _ns.NavigateTo<SearchPage>();
    }
}
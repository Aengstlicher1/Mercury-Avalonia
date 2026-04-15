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
    private readonly INavigationService _ns;
    private readonly SearchPage _sp;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainWindowViewModel>();
        _sp = App.Services.GetRequiredService<SearchPage>();
        _ns = App.Services.GetRequiredService<INavigationService>();
        
        _ns.Navigation = Navigator;
    }

    private void SearchBox_GotFocus(object? sender, FocusChangedEventArgs e)
    {
        _ns.NavigateTo(_sp);
    }
}
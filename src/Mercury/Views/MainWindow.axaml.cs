using System;
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
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainWindowViewModel>();
        
        _ns = App.Services.GetRequiredService<INavigationService>();
        Loaded += (_ , _) => _ns.SetHost(Navigator);
        Closed += OnClosed;
    }

    private void OnClosed(object? s, EventArgs e)
    {
        var serviceBases = App.Services.GetServices<IServiceBase>();
        foreach (var service in serviceBases)
        {
            service.OnExit();
        }
    }
    
    private void SearchBox_GotFocus(object? sender, FocusChangedEventArgs e)
    {
        _ns.NavigateTo<SearchPage>();
    }
}
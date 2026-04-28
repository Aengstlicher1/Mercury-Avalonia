using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Mercury.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Views;

public partial class PlayingPage : ContentPage
{
    public PlayingPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<PlayingPageViewModel>();
    }
}
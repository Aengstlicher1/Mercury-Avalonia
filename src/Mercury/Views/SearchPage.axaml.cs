using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mercury.Services;
using Mercury.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Views;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<SearchPageViewModel>();
    }
}
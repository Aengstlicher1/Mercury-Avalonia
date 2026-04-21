using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mercury.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Views;

public partial class ExplorePage : ContentPage
{
    public ExplorePage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ExplorePageViewModel>();
    }
}
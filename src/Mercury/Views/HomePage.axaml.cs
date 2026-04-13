using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mercury.ViewModels;

namespace Mercury.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomePageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
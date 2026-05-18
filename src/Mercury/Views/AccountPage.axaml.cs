using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Shared;
using Mercury.Services.Implementations;
using Mercury.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Views;

public partial class AccountPage : ContentPage
{
    private readonly IAccountService _accountService;
    private NativeWebViewCookieManager? _cookieManager;
 
    public static string LoginUrl { get; } =
        "https://accounts.google.com/ServiceLogin?service=youtube&continue=https%3A%2F%2Fmusic.youtube.com%2F";
    
    public AccountPage()
    {
        InitializeComponent();
        _accountService = App.Services.GetRequiredService<IAccountService>();
    }
    
    private void OnAdapterCreated(object? sender, WebViewAdapterEventArgs e)
    {
        _cookieManager = WebView.TryGetCookieManager();
        
        Console.WriteLine(_cookieManager is null
            ? "No cookie manager available on this platform/backend."
            : "Cookie manager acquired.");
    }
    
    private async void OnNavigationStarted(object? sender, WebViewNavigationStartingEventArgs e)
    {
        try
        {
            if (e.Request is null)
                return;

            var uri = e.Request;
            var host = uri.Host.ToLowerInvariant();
            
            if (!host.Contains("youtube.com") ||
                !uri.AbsoluteUri.StartsWith("https://music.youtube.com", StringComparison.OrdinalIgnoreCase))
                return;

            if (_cookieManager is null)
            {
                Console.WriteLine("Cookie manager is not available; skipping AddAccount.");
                return;
            }
            await _accountService.AddAccount(_cookieManager);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

}
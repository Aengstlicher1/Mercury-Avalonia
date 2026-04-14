using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Mercury.Models;
using Mercury.Resources;
using Mercury.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Services;

public partial class NavigationService : INavigationService
{
    public INavigation? Navigation { get; set; }
    
    public PageInfo? CurrentPageInfo { get; set; }
    public Page? CurrentPage => CurrentPageInfo?.Page;

    public ContentPage[] Pages { get; } = new ContentPage[3];
    
    public PageInfo[] PageInfos { get; }

    public NavigationService()
    {
        Pages[0] = App.Services.GetRequiredService<HomePage>();
        Pages[1] = new ContentPage
        {
            Name = "Explore",
            Background = Brushes.Transparent,
            Content = new Button() { Content = "Filler - Explore" },
            [NavigationPage.HasNavigationBarProperty] = false
        };
        Pages[2] = new ContentPage
        {
            Name = "Library",
            Background = Brushes.Transparent,
            Content = new Button() { Content = "Filler - Library" },
            [NavigationPage.HasNavigationBarProperty] = false
        };
        
        PageInfos = Pages.Select(p => new PageInfo
        {
            Name = p.Name!,
            Page = p
        }).ToArray();
    }

    public async Task NavigateTo(Page page, bool slideLeft = false)
    {
        if (Navigation != null)
        {
            var transition = new DirectionalPageSlide()
            {
                Duration = TimeSpan.FromMilliseconds(160),
                SlideLeft = slideLeft
            };
            
            await Navigation.ReplaceAsync(page, transition);
        }
    }
    
    [RelayCommand]
    public async Task NavigateTo(PageInfo pageInfo)
    {
        foreach (var p in PageInfos)
            p.IsSelected = false;
        pageInfo.IsSelected = true;

        bool slideleft;
        if (PageInfos.Contains(pageInfo))
        {
            int currentIndex = PageInfos.IndexOf(CurrentPageInfo);
            int targetIndex = PageInfos.IndexOf(pageInfo);
                
            slideleft = targetIndex < currentIndex;
        }
        else
        {
            slideleft = false;
        }
        
        await NavigateTo(pageInfo.Page, slideleft);
        CurrentPageInfo = pageInfo;
    }
}
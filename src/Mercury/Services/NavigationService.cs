using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Mercury.Models;
using Mercury.Resources;
using Mercury.Views;

namespace Mercury.Services;

public partial class NavigationService : INavigationService
{
    public INavigation? Navigation { get; set; }
    
    public PageInfo? CurrentPageInfo { get; set; }
    public Page? CurrentPage => CurrentPageInfo?.Page;
    
    public static ContentPage[] Pages { get; } =
    [
        new ContentPage
        {
            Name = "Home", 
            Content = new Button(){ Content = "Test - Home" }, 
            [NavigationPage.HasNavigationBarProperty] = false
        },
        new ContentPage
        {
            Name = "Explore", 
            Content = new Button(){ Content = "Test - Explore" }, 
            [NavigationPage.HasNavigationBarProperty] = false
        },new ContentPage
        {
            Name = "Library", 
            Content = new Button(){ Content = "Test - Library" }, 
            [NavigationPage.HasNavigationBarProperty] = false
        }
    ];
    
    public PageInfo[] PageInfos { get; } = Pages.Select(p => new PageInfo
    {
        Name = p.Name,
        Page = p
    }).ToArray();

    [RelayCommand]
    public async Task NavigateTo(PageInfo pageInfo)
    {
        foreach (var p in PageInfos)
            p.IsSelected = false;
        pageInfo.IsSelected = true;

        if (Navigation != null)
        {
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

            var transition = new DirectionalPageSlide()
            {
                Duration = TimeSpan.FromMilliseconds(160),
                SlideLeft = slideleft
            };
            
            CurrentPageInfo = pageInfo;
            _ = Navigation.ReplaceAsync(pageInfo.Page, transition);
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
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
    
    public PageInfo? CurrentPageInfo => PageInfos.FirstOrDefault(pi => pi.Page == CurrentPage);
    public Page? CurrentPage { get; set; }

    public ContentPage[] Pages { get; } = new ContentPage[2];
    
    public PageInfo[] PageInfos { get; }

    public NavigationService()
    {
        Pages[0] = App.Services.GetRequiredService<HomePage>();
        Pages[1] = new ContentPage
        {
            Name = "Library",
            Background = Brushes.Transparent,
            Content = new TextBlock() { Text = "Filler - Library", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment =  HorizontalAlignment.Center },
            [NavigationPage.HasNavigationBarProperty] = false
        };
        
        PageInfos = Pages.Select(p => new PageInfo(p)).ToArray();
    }

    public void NavigateTo(Page page, bool slideLeft = false)
    {
        if (Navigation != null)
        {
            var transition = new DirectionalPageSlide()
            {
                Duration = TimeSpan.FromMilliseconds(160),
                SlideLeft = slideLeft
            };
            
            _ = Navigation.ReplaceAsync(page, transition);
            CurrentPage = page;
        }
    }
    
    [RelayCommand]
    public void NavigateTo(PageInfo pageInfo)
    {
        foreach (var p in PageInfos)
        {
            p.IsSelected = false;
        }
        pageInfo.IsSelected = true;

        bool slideLeft;
        if (CurrentPage is SearchPage)
        {
            slideLeft = true;
        }
        else if (PageInfos.Contains(pageInfo))
        {
            int currentIndex = PageInfos.IndexOf(CurrentPageInfo);
            int targetIndex = PageInfos.IndexOf(pageInfo);
                
            slideLeft = targetIndex < currentIndex;
        }
        else
        {
            slideLeft = false;
        }
        
        NavigateTo(pageInfo.Page, slideLeft);
        CurrentPage = pageInfo.Page;
    }
}
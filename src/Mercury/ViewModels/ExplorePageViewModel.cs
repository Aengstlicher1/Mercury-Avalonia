using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Core.Models.Explore;
using Microsoft.VisualBasic;

namespace Mercury.ViewModels;

public partial class ExplorePageViewModel : ViewModelBase
{
    private ExploreFeed _feed = null!;
    
    [ObservableProperty]
    private ObservableCollection<Album> _releases = new();
    
    [ObservableProperty]
    private ObservableCollection<Genre> _genres = new();
    
    [ObservableProperty]
    private ObservableCollection<Genre> _trending = new();
    
    [ObservableProperty]
    private ObservableCollection<Genre> _musicVideos = new();
    
    public ExplorePageViewModel()
    {
        Task.Run(async () =>
        {
            _feed = await YoutubeMusic.Browse.GetExploreFeedAsync();
            LoadFeed();
        });
    }
    
    private void LoadFeed()
    {
        Releases.Clear();
        foreach (var item in _feed.Releases.Content)
        {
            Releases.Add(item);
        }
        
        Genres.Clear();
        foreach (var item in _feed.Genres.Content)
        {
            Genres.Add(item);
        }
    }
}
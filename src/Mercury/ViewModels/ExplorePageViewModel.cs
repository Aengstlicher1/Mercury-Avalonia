using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Core.Models.Explore;
using Microsoft.VisualBasic;

namespace Mercury.ViewModels;

public partial class ExplorePageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Album> _releases = new();
    
    [ObservableProperty]
    private ObservableCollection<Genre> _genres = new();
    
    [ObservableProperty]
    private ObservableCollection<Track> _trending = new();
    
    [ObservableProperty]
    private ObservableCollection<Video> _musicVideos = new();
    
    public ExplorePageViewModel()
    {
        LoadFeedCommand.ExecuteAsync(null);
    }
    
    [RelayCommand]
    private async Task LoadFeedAsync()
    {
        var feed = await YoutubeMusic.Browse.GetExploreFeedAsync();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Releases = new ObservableCollection<Album>(feed.Releases.Content);
            Genres = new ObservableCollection<Genre>(feed.Genres.Content);
            Trending = new ObservableCollection<Track>(feed.Trending.Content);
            MusicVideos = new ObservableCollection<Video>(feed.NewMusicVideos.Content);
        });
    }
}
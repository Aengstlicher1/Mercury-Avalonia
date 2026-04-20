using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mercury.Core.Models;
using Mercury.Models;
using Mercury.Services;
using Microsoft.Extensions.DependencyInjection;
using static Mercury.Core.Models.Enums;

namespace Mercury.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private HttpClient _client = new HttpClient();
    
    public IEnumerable<SearchFilter> SearchFilters =>
        Enum.GetValues(typeof(SearchFilter)).Cast<SearchFilter>()
            .Where(s =>
                s != SearchFilter.Episodes &&
                s != SearchFilter.Profiles
            );
    
    [ObservableProperty]
    private Track? _currentTrack;
    
    [ObservableProperty]
    private bool _isPlaying;
    
    [ObservableProperty]
    private float _currentTrackPosition;

    [ObservableProperty] 
    private int _volume = 60;
    
    [ObservableProperty]
    private IImage? _currentBackgroundImage;
    
    
    [ObservableProperty]
    private string _searchText = "";
    
    [ObservableProperty]
    private SearchFilter _searchFilter = SearchFilter.All;
    
    private readonly INavigationService _navigationService;
    private readonly ISearchService _searchService;
    private readonly IPlayerService _playerService;
    
    public PageInfo[] PageInfos => _navigationService.PageInfos;
    public IRelayCommand<PageInfo> NavigateToCommand =>  _navigationService.NavigateToCommand;
    
    public MainWindowViewModel()
    {
        _navigationService = App.Services.GetRequiredService<INavigationService>();
        _searchService = App.Services.GetRequiredService<ISearchService>();
        _playerService = App.Services.GetRequiredService<IPlayerService>();
        
        _navigationService.NavigateTo(_navigationService.PageInfos[0]);

        _playerService.CurrentTrackChanged += track => CurrentTrack = track;
        _playerService.PositionChanged += pos => CurrentTrackPosition = pos; // update UI
        _playerService.VolumeChanged += vol => Volume = vol;
        _playerService.PlayingChanged += playing => IsPlaying = playing;
    }
    
    private async Task<Bitmap> LoadTrackImageAsync(string url)
    {
        var bytes = await _client.GetByteArrayAsync(url);
        using var ms = new MemoryStream(bytes);
        return new Bitmap(ms);
    }


    [RelayCommand]
    private void TogglePlay()
    {
        if (CurrentTrack != null)
        {
            switch (IsPlaying)
            {
                case true:  _playerService.PausePlayblack(); break;
                case false: _playerService.StartPlayblack(); break;
            }
        }
    }

    [RelayCommand]
    private void SkipForward()
        => _playerService.SkipForward();
    
    [RelayCommand]
    private void SkipBack()
        => _playerService.SkipBack();
    
    
    partial void OnCurrentTrackChanged(Track? value)
    {
        Task.Run(async () =>
        {
            CurrentBackgroundImage = await LoadTrackImageAsync(value!.Thumbnails.LowestRes.Url);
        });
    }

    partial void OnSearchTextChanged(string value)
    {
        _searchService.SearchQuery = value;
    }

    partial void OnSearchFilterChanged(SearchFilter value)
    {
        _searchService.SearchFilter = value;
    }

    partial void OnCurrentTrackPositionChanged(float value)
    {
        _playerService.Position = value;
    }

    partial void OnVolumeChanged(int value)
    {
        _playerService.Volume = value;
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconPacks.Avalonia.MaterialDesign;
using Mercury.Core.Models;
using Mercury.Models;
using Mercury.Services;
using Mercury.Services.Interfaces;
using Mercury.Views;
using Microsoft.Extensions.DependencyInjection;
using static System.Enum;
using static Mercury.Core.Models.Enums;

namespace Mercury.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly HttpClient _client = new HttpClient();
    
    public IEnumerable<SearchFilter> SearchFilters =>
        GetValues(typeof(SearchFilter)).Cast<SearchFilter>()
            .Where(s =>
                s != SearchFilter.Episodes &&
                s != SearchFilter.Profiles
            );
    
    [ObservableProperty]
    public partial Track? CurrentTrack { get; set; }
    
    [ObservableProperty]
    public partial bool IsPlaying { get; set; }

    [ObservableProperty]
    public partial float CurrentTrackPosition { get; set; }

    [ObservableProperty]
    public partial int Volume { get; set; }

    [ObservableProperty]
    public partial RepeatState RepeatState { get; set; }

    [ObservableProperty]
    public partial IImage? CurrentBackgroundImage { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial SearchFilter SearchFilter { get; set; } = SearchFilter.All;
    public INavigationService NavigationService { get; }
    private readonly ISearchService _searchService;
    private readonly IPlayerService _playerService;
    
    public IRelayCommand<PageDescriptor> NavigateToCommand => NavigationService.NavigateToCommand;
    
    public MainWindowViewModel()
    {
        NavigationService = App.Services.GetRequiredService<INavigationService>();
        _searchService = App.Services.GetRequiredService<ISearchService>(); 
        _playerService = App.Services.GetRequiredService<IPlayerService>();
        
        _playerService.CurrentTrackChanged += track => CurrentTrack = track;
        _playerService.PositionChanged += pos => CurrentTrackPosition = pos;
        _playerService.VolumeChanged += vol => Volume = vol;
        _playerService.PlayingChanged += playing => IsPlaying = playing;
        _playerService.RepeatStateChanged += state => RepeatState = state;
        RepeatState = _playerService.RepeatState;
        Volume = _playerService.Volume;
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

    [RelayCommand]
    private void SwitchRepeatState()
    {
        switch (RepeatState)
        {
            case RepeatState.NoRepeat:
                RepeatState = RepeatState.RepeatSingle;
                break;
            
            case RepeatState.RepeatSingle:
                RepeatState = RepeatState.RepeatAll;
                break;
            
            case RepeatState.RepeatAll:
                RepeatState = RepeatState.Shuffle;
                break;
            
            case RepeatState.Shuffle:
                RepeatState = RepeatState.NoRepeat;
                break;
        }
    }

    [RelayCommand]
    private void EnterPlaying()
    {
        NavigationService.NavigateTo<PlayingPage>();
    }
    
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

    partial void OnRepeatStateChanged(RepeatState value)
    {
        _playerService.RepeatState = value;
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core.Models;
using Mercury.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class PlayingPageViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService;
    private readonly ILyricService _lyricService;
    
    public PlayingPageViewModel()
    {
        _playerService = App.Services.GetRequiredService<IPlayerService>();
        _lyricService = App.Services.GetRequiredService<ILyricService>();
        
        _playerService.CurrentTrackChanged += track =>
        {
            CurrentTrack = track;
        };
        _lyricService.LyricsChanged += lyrics =>
        {
            Lyrics = lyrics;
        };
        
        CurrentTrack = _playerService.CurrentTrack;
    }
    
    [ObservableProperty]
    private Track? _currentTrack;
    
    [ObservableProperty]
    private Lyrics? _lyrics;


    partial void OnCurrentTrackChanged(Track? value)
    {
        _playerService.CurrentTrack = value;
    }
}
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mercury.Core.Models;
using Mercury.Resources.Behaviors;
using Mercury.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class LyricsViewerViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService;
    private readonly ILyricService  _lyricService;
    private Lyrics? _lyrics;
    private bool    _loadingLyrics;

    public LyricsViewerViewModel()
    {
        _playerService = App.Services.GetRequiredService<IPlayerService>();
        _lyricService  = App.Services.GetRequiredService<ILyricService>();

        _playerService.CurrentTrackChanged += track => _currentTrack = track;
        _lyricService.LyricsChanged += LoadLyrics;
        _playerService.PositionChanged += TrackPositionChanged;
        _currentTrack = _playerService.CurrentTrack;
        LoadLyrics(_lyricService.Lyrics);
    }

    // Not exposed as ObservableProperty — only used internally for timing
    private Track? _currentTrack;

    [ObservableProperty] private ObservableCollection<LyricLineViewModel> _lyricLines = [];
    [ObservableProperty] private int  _currentLineIndex = -1;
    [ObservableProperty] private bool _autoScroll       = true;
    [ObservableProperty] private bool _useSyncedLyrics  = true;
    [ObservableProperty] private bool _hasSyncedLyrics  = false;

    [RelayCommand]
    private void ToggleAutoScroll() => SetAutoScroll(!AutoScroll);

    [RelayCommand]
    private void SetAutoScroll(bool value)
    {
        AutoScroll = value;
            
        /* Reset the currentLineIndex to force the AutoScroll to happen immediatly */
        int current = CurrentLineIndex;
        CurrentLineIndex = -1;
        CurrentLineIndex = current;
    }
    
    [RelayCommand]
    private void ToggleLyricMode()
    {
        if (!HasSyncedLyrics) return;
        UseSyncedLyrics = !UseSyncedLyrics;
    }

    [RelayCommand]
    private void GoToLine(LyricLine line)
    {
        if (_currentTrack is not null && LyricLines.Any(l => l.Line == line))
        {
            var pos = (line.Timing + TimeSpan.FromMilliseconds(5)) / _currentTrack.DurationTimeSpan;
            _playerService.Position = (float)pos;
        }
    }
    
    private void LoadLyrics(Lyrics? lyrics)
    {
        if (lyrics is not null)
        {
            _loadingLyrics  = true;
            _lyrics         = lyrics;
            HasSyncedLyrics = lyrics?.SyncedLyrics?.Count > 0;

            if (!HasSyncedLyrics)
                UseSyncedLyrics = false;

            _loadingLyrics = false;
            RebuildLines();
        }
    }

    private void RebuildLines()
    {
        LyricLines.Clear();
        CurrentLineIndex = -1;

        if (_lyrics is null) return;

        var source = UseSyncedLyrics
            ? _lyrics.SyncedLyrics
            : _lyrics.PlainLyrics;

        if (source is null) return;

        foreach (var line in source)
            LyricLines.Add(new LyricLineViewModel(line));
    }

    private void TrackPositionChanged(float position)
    {
        Debug.WriteLine($"TrackPositionChanged: position={position}, _currentTrack={_currentTrack?.Title}, UseSyncedLyrics={UseSyncedLyrics}");
        
        if (UseSyncedLyrics && _currentTrack is not null && LyricLines.Count > 0)
        {
            var currentTime = _currentTrack.DurationTimeSpan * position;
            int newIndex    = -1;

            for (int i = LyricLines.Count - 1; i >= 0; i--)
            {
                if (LyricLines[i].Timing <= currentTime)
                {
                    newIndex = i;
                    break;
                }
            }

            if (newIndex == CurrentLineIndex) return;

            Dispatcher.UIThread.Post(() =>
            {
                for (int i = 0; i < LyricLines.Count; i++)
                {
                    LyricLines[i].State = i < newIndex  ? LyricState.Past
                        : i == newIndex ? LyricState.Current
                        : LyricState.Upcoming;
                }
                CurrentLineIndex = newIndex;
            });
        }
    }

    partial void OnUseSyncedLyricsChanged(bool value)
    {
        if (!_loadingLyrics)
            RebuildLines();
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core;
using Mercury.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Services;

public partial class LyricService : ServiceBase, ILyricService
{
    private readonly IPlayerService _playerService;
    
    public LyricService()
    {
        _playerService = App.Services.GetRequiredService<IPlayerService>();
        _playerService.CurrentTrackChanged += async track =>
        {
            Lyrics = await YoutubeMusic.Lyrics.GetLyricsAsync(track);
        };

        Task.Run(async () => Lyrics = await YoutubeMusic.Lyrics.GetLyricsAsync(_playerService.CurrentTrack!));
    }
    
    
    [ObservableProperty]
    private Lyrics? _lyrics;
    
    public event Action<Lyrics?>? LyricsChanged;


    partial void OnLyricsChanged(Lyrics? value)
    {
        LyricsChanged?.Invoke(value);
    }

    public override void OnExit()
    {
        
    }
}
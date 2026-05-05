using System;
using DiscordRPC;
using Mercury.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Services;

public class DiscordService : IDiscordService
{
    private const string ClientId = "1500213810898927676";
    private readonly DiscordRpcClient _client = new DiscordRpcClient(ClientId);
    private readonly IPlayerService _playerService = App.Services.GetRequiredService<IPlayerService>();
    private DateTime _lastPresenceUpdate = DateTime.MinValue;
    
    public void Initialize()
    {
        if (!_client.IsInitialized)
        {
            _client.Initialize();
            _playerService.PositionChanged += _
                => UpdatePresence();
            _playerService.CurrentTrackChanged += _
                => UpdatePresence();
        }
    }
    
    private void UpdatePresence()
    {
        /* Return early if less than one second since the last update has passed to not spam discord. */
        if (DateTime.Now - _lastPresenceUpdate < TimeSpan.FromSeconds(1))
            return;
        _lastPresenceUpdate = DateTime.Now;
        
        var track = _playerService.CurrentTrack;
        
        if (track != null)
        {
            _client.SetPresence(new RichPresence
            {
                StatusDisplay = StatusDisplayType.Details,
                Details = track.Title,
                DetailsUrl = track.Url,
                State = $"by {track.Artist}",
                Type = ActivityType.Listening,
                Assets = new Assets
                {
                    LargeImageUrl = track.Thumbnails.HighestRes.Url
                },
                Buttons = 
                [
                    new Button()
                    {
                        Label = "Listen too",
                        Url = track.Url
                    }
                ]
            });
        }
    }
    
    
    
    public void ClearPresence()
    {
        _client.ClearPresence();
    }
    
    public void OnExit()
    {
        ClearPresence();
        _client.Deinitialize();
    }
}
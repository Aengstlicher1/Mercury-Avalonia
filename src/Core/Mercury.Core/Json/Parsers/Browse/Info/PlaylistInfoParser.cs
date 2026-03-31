using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Browse.Info
{
    internal static class PlaylistInfoParser
    {
        public static PlaylistInfo Parse(JElement renderer, JArray tracks)
        {
            Collection<PlaylistTrack> playlistTracks = new();
            foreach (var track in tracks)
            {
                playlistTracks.Add(
                    ParsePlaylistTrack(track.Get("musicResponsiveListItemRenderer"))
                );
            }

            return new PlaylistInfo()
            {
                Tracks = playlistTracks.ToArray()
            };
        }

        private static PlaylistTrack ParsePlaylistTrack(JElement renderer)
        {
            var category = renderer
                .Get("thumbnail")
                .Get("musicThumbnailRenderer")
                .Get("thumbnail")
                .Get("thumbnails");

            return null;
        }

        private static SongPlaylistTrack ParseSongPlaylistTrack(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);

            return new SongPlaylistTrack()
            {
                Id = renderer.Get("playlistItemData").Get("videoId").AsString().Or(string.Empty),
                Title = FlexColumnParser.Parse(flex, 0),
                Duration = FixedColumnParser.Parse(FixedColumnParser.GetFix(renderer), 0),
                Thumbnails = thumbnails
            };
        }
    }
}

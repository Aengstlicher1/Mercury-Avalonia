using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Models
{
    public abstract class MediaInfo
    {

    }

    public class PlaylistInfo : MediaInfo
    {
        public PlaylistTrack[] Tracks { get; set; } = Array.Empty<PlaylistTrack>();
        public int TracksCount => Tracks.Length;
    }

    public abstract class PlaylistTrack : Track;

    public class SongPlaylistTrack : PlaylistTrack
    {
        public string Album { get; set; } = string.Empty;
    }

    public class VideoPlaylistTrack : PlaylistTrack
    {
        public string Views { get; set; } = string.Empty;
    }

    public class EpisodePlaylistTrack : PlaylistTrack;
}

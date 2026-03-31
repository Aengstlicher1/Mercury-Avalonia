using Mercury.Core.Json;
using Mercury.Core.Json.Parsers.Browse;
using Mercury.Core.Json.Parsers.Browse.Info;
using Mercury.Core.Models;
using Mercury.Core.Network;
using Mercury.Core.Utils;
using static Mercury.Core.Models.Enums;

namespace Mercury.Core.Services
{
    public sealed class BrowseService
    {
        //\\ Browse Endpoint for normal Model //\\

        public async Task<Media> GetAsync(string id, MediaCategory category, CancellationToken cToken = default)
        {
            return category switch
            {
                MediaCategory.Song or 
                MediaCategory.Video         => await HandleNext(id, category, cToken),
                MediaCategory.Episode       => await HandleEpisode(id, cToken),
                MediaCategory.Artist        => await HandleArtists(id, cToken),
                MediaCategory.Profile       => await HandleProfiles(id, cToken),
                MediaCategory.Album or 
                MediaCategory.Playlist or
                MediaCategory.Podcast       => await HandleBrowse(id, category, cToken),
                _                           => null!
            };
        }

        private async Task<Media> HandleNext(string videoId, MediaCategory category, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                throw new ArgumentNullException("videoId");

            Dictionary<string, object?> payload = new()
            {
                {"videoId" , videoId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Next, payload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            using IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("contents")
                .Get("singleColumnMusicWatchNextResultsRenderer")
                .Get("tabbedRenderer")
                .Get("watchNextTabbedResultsRenderer")
                .Get("tabs")
                .GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("musicQueueRenderer")
                .Get("content")
                .Get("playlistPanelRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("playlistPanelVideoRenderer");

            Media result = category switch
            {
                MediaCategory.Song      => SongParser.Parse(renderer),
                MediaCategory.Video     => VideoParser.Parse(renderer),
                _ => null!
            };

            return result;
        }

        private async Task<Media> HandleBrowse(string browseId, MediaCategory category, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(browseId))
                throw new ArgumentNullException("browseId");

            Dictionary<string, object?> payload = new()
            {
                {"browseId" , browseId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            using IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("contents")
                .Get("twoColumnBrowseResultsRenderer")
                .Get("tabs")
                .GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("sectionListRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("musicResponsiveHeaderRenderer");

            Media result = category switch 
            {
                MediaCategory.Album => AlbumParser.Parse(renderer, browseId),
                MediaCategory.Playlist => PlaylistParser.Parse(renderer, browseId),
                MediaCategory.Podcast => PodcastParser.Parse(renderer, browseId),
                _ => null!
            };

            return result;
        }

        private async Task<Profile> HandleProfiles(string browseId, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(browseId))
                throw new ArgumentNullException("browseId");

            Dictionary<string, object?> payload = new()
            {
                {"browseId" , browseId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            using IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("header")
                .Get("musicVisualHeaderRenderer");

            return ProfileParser.Parse(renderer, browseId);
        }

        private async Task<Artist> HandleArtists(string browseId, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(browseId))
                throw new ArgumentNullException("browseId");

            Dictionary<string, object?> payload = new()
            {
                {"browseId" , browseId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            using IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("header")
                .Get("musicImmersiveHeaderRenderer");

            return ArtistParser.Parse(renderer, browseId);
        }

        private async Task<Episode> HandleEpisode(string id, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("id");

            Dictionary<string, object?> nextPayload = new()
            {
                {"videoId" , id }
            };
            Dictionary<string, object?> browsePayload = new()
            {
                {"browseId" , "MPED" + id }
            };

            var nextResponse = await RequestHandler.PostAsync(Endpoints.Next, nextPayload, ClientType.WebMusic, cToken);
            var browseResponse = await RequestHandler.PostAsync(Endpoints.Browse, browsePayload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            nextResponse.ParseJson(out var nextJson);
            browseResponse.ParseJson(out var browseJson);

            var nextRenderer = nextJson
                .Get("contents")
                .Get("singleColumnMusicWatchNextResultsRenderer")
                .Get("tabbedRenderer")
                .Get("watchNextTabbedResultsRenderer")
                .Get("tabs")
                .GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("musicQueueRenderer")
                .Get("content")
                .Get("playlistPanelRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("playlistPanelVideoRenderer");

            var browseRenderer = browseJson
                .Get("contents")
                .Get("twoColumnBrowseResultsRenderer")
                .Get("tabs")
                .GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("sectionListRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("musicResponsiveHeaderRenderer");

            return EpisodeParser.Parse(nextRenderer, browseRenderer);
        }



        //\\ Browse Endpoint for Info-Model //\\
        public async Task<MediaInfo> GetInfoAsync(string id, MediaCategory category, CancellationToken cToken = default)
        {
            return category switch
            {
                MediaCategory.Playlist  => await HandlePlalistInfo(id, cToken),
                _                       => null!
            };
        }

        private async Task<PlaylistInfo> HandlePlalistInfo(string id, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("id");

            Dictionary<string, object?> payload = new()
            {
                {"browseId" , id }
            };

            var response = await RequestHandler.GetAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

            IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("contents")
                .Get("twoColumnBrowseResultsRenderer")
                .Get("secondaryContents")
                .Get("sectionListRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("musicPlaylistShelfRenderer");

            var tracks = renderer
                .Get("contents")
                .AsArray()
                .Or(JArray.Empty);

            return PlaylistInfoParser.Parse(renderer, tracks);
        }
    }
}

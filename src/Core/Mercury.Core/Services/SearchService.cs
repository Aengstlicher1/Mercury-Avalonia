using Mercury.Core.Json;
using Mercury.Core.Json.Parsers;
using Mercury.Core.Json.Parsers.Search;
using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using Mercury.Core.Network;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace Mercury.Core.Services
{
    public sealed class SearchService
    {
        public async Task<Collection<Media>?> SearchAsync(
            string query,
            bool ignoreSpelling = true,
            CancellationToken cToken = default
        )
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                    throw new ArgumentNullException("query");

                Dictionary<string, object?> payload = new()
                {
                    { "query", query }
                };

                var response = await RequestHandler.PostAsync(Endpoints.Search, payload, ClientType.WebMusic, cToken);

                cToken.ThrowIfCancellationRequested();

                using IDisposable _ = response.ParseJson(out var json);

                var renderers = json
                    .Get("contents")
                    .Get("tabbedSearchResultsRenderer")
                    .Get("tabs").GetAt(0)
                    .Get("tabRenderer")
                    .Get("content")
                    .Get("sectionListRenderer")
                    .Get("contents");

                var topResult = renderers
                    .GetAt(1)
                    .Get("musicCardShelfRenderer");

                var normalResults = renderers
                    .GetAt(2).Get("musicShelfRenderer")
                    .Get("contents")
                    .AsArray()
                    .Or(JArray.Empty);

                Collection<Media> results = new Collection<Media>();
                foreach (var result in normalResults)
                {
                    cToken.ThrowIfCancellationRequested();

                    var renderer = result.Get("musicResponsiveListItemRenderer");
                    var category = CategoryParser.Parse(renderer);

                    results.Add
                    (
                        category switch
                        {
                            Enums.MediaCategory.Song        => SongParser.Parse(renderer),
                            Enums.MediaCategory.Video       => VideoParser.Parse(renderer),
                            Enums.MediaCategory.Artist      => ArtistParser.Parse(renderer),
                            Enums.MediaCategory.Album       => AlbumParser.Parse(renderer),
                            Enums.MediaCategory.Playlist    => PlaylistParser.Parse(renderer),
                            Enums.MediaCategory.Episode     => EpisodeParser.Parse(renderer),
                            Enums.MediaCategory.Profile     => ProfileParser.Parse(renderer),
                            Enums.MediaCategory.Podcast    => PodcastParser.Parse(renderer),
                            _ => null!
                        }
                    );
                }

                return results;
            }
            catch (TaskCanceledException) { }
            return null!;
        }


        public async Task<Collection<Media>?> SearchCategoryAsync(
            string query,
            Enums.SearchFilter filter,
            bool ignoreSpelling = true,
            CancellationToken cToken = default)
        {
            if (filter is Enums.SearchFilter.All)
                return await SearchAsync(query, cToken:cToken);

            try 
            {
                if (string.IsNullOrEmpty(query))
                    throw new ArgumentNullException("query");

                Dictionary<string, object?> payload = new()
                {
                    { "query", query },
                    { "params", filter.ToParam() }
                }; 

                var startTime = DateTime.Now;
                var response = await RequestHandler.PostAsync(Endpoints.Search, payload, ClientType.WebMusic, cToken);
                Debug.WriteLine($"Core: \"Sending Search Request\" took { DateTime.Now - startTime}");

                using IDisposable _ = response.ParseJson(out var json);

                var shelfResults = json
                    .Get("contents")
                    .Get("tabbedSearchResultsRenderer")
                    .Get("tabs").GetAt(0)
                    .Get("tabRenderer")
                    .Get("content")
                    .Get("sectionListRenderer")
                    .Get("contents")
                    .GetAt(1)
                    .Get("musicShelfRenderer")
                    .Get("contents")
                    .AsArray()
                    .Or(JArray.Empty);

                Collection<Media> results = new Collection<Media>();
                foreach (var result in shelfResults)
                {
                    cToken.ThrowIfCancellationRequested();

                    var renderer = result.Get("musicResponsiveListItemRenderer");
                    var category = CategoryParser.Parse(renderer);

                    results.Add
                    (
                        filter switch
                        {
                            Enums.SearchFilter.Songs                => Json.Parsers.Search.Category.SongParser.Parse(renderer),
                            Enums.SearchFilter.Videos               => Json.Parsers.Search.Category.VideoParser.Parse(renderer),
                            Enums.SearchFilter.Artists              => ArtistParser.Parse(renderer),
                            Enums.SearchFilter.Albums               => AlbumParser.Parse(renderer),
                            Enums.SearchFilter.FeaturedPlaylists    => Json.Parsers.Search.Category.PlaylistParser.Parse(renderer),
                            Enums.SearchFilter.CommunityPlaylists   => Json.Parsers.Search.Category.PlaylistParser.Parse(renderer),
                            Enums.SearchFilter.Episodes             => Json.Parsers.Search.Category.EpisodeParser.Parse(renderer),
                            Enums.SearchFilter.Profiles             => ProfileParser.Parse(renderer),
                            Enums.SearchFilter.Podcasts             => Json.Parsers.Search.Category.PodcastParser.Parse(renderer),
                            _ => null!
                        }
                    );
                }

                return results;
            }
            catch (TaskCanceledException) { }
            return null;
        }
    }
}

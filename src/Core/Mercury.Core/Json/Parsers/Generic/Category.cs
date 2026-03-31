using Mercury.Core.Models;
using static Mercury.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class CategoryParser
    {
        internal static MediaCategory Parse(JElement renderer)
        {
            var flex = FlexColumnParser.GetFlex(renderer);
            var category = FlexColumnParser.Parse(flex, 1);

            string pageType = RunsParser.GetRuns(flex[1].Get("musicResponsiveListItemFlexColumnRenderer").Get("text"))[2]
                .Get("navigationEndpoint")
                .Get("browseEndpoint")
                .Get("browseEndpointContextSupportedConfigs")
                .Get("browseEndpointContextMusicConfig")
                .Get("pageType")
                .AsString()
                .Or(string.Empty);
                
            if (pageType.Contains("MUSIC_PAGE_TYPE_PODCAST_SHOW_DETAIL_PAGE"))
                return MediaCategory.Episode;

            return category switch
            {
                "Song" => MediaCategory.Song,
                "Video" => MediaCategory.Video,
                "Playlist" => MediaCategory.Playlist,
                "Artist" => MediaCategory.Artist,
                "Episode" => MediaCategory.Episode,
                "EP" or "Album" or "Single" => MediaCategory.Album,
                "Profile" => MediaCategory.Profile,
                "Podcast" => MediaCategory.Podcast,
                _ => MediaCategory.None
            };
        }
    }
}

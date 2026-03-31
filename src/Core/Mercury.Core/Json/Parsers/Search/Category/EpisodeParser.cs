using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Search.Category
{
    internal static class EpisodeParser
    {
        internal static Episode Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);

            return new Episode()
            {
                Id = renderer.Get("playlistItemData").Get("videoId").AsString().Or(string.Empty),
                Title = FlexColumnParser.Parse(flex, 0),
                Date = FlexColumnParser.Parse(flex, 1),
                PodcastName = FlexColumnParser.Parse(flex, 1, 2),
                Thumbnails = thumbnails,
            };
        }
    }
}

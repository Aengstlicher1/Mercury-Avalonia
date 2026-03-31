using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;


namespace Mercury.Core.Json.Parsers.Search
{
    internal static class PodcastParser
    {
        internal static Podcast Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);

            return new Podcast()
            {
                Id = IdParser.ParseBrowse(renderer),
                Title = FlexColumnParser.Parse(flex, 0),
                Artists = FlexColumnParser.Parse(flex, 1, 2),
                Thumbnails = thumbnails,
            };
        }
    }
}

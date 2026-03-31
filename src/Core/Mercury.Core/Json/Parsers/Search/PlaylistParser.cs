using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Search
{
    internal static class PlaylistParser
    {
        internal static Playlist Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);

            return new Playlist()
            {
                Id = IdParser.ParseBrowse(renderer),
                Title = FlexColumnParser.Parse(flex,0),
                Artists = FlexColumnParser.Parse(flex, 1, 2),
                Thumbnails = thumbnails,
                Views = FlexColumnParser.Parse(flex, 1, 4)
            };
        }
    }
}

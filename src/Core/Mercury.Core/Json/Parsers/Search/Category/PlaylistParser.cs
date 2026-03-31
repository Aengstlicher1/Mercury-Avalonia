using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Search.Category
{
    internal static class PlaylistParser
    {
        internal static Playlist Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);

            var info = FlexColumnParser.Parse(flex, 1, 2);

            return new Playlist()
            {
                Id = IdParser.ParseBrowse(renderer),
                Title = FlexColumnParser.Parse(flex, 0),
                Artists = FlexColumnParser.Parse(flex, 1),
                Views = info.Contains("views") ? info : string.Empty,
                ItemCount = info.Contains("views") ? string.Empty : info,
                Thumbnails = thumbnails
            };
        }
    }
}

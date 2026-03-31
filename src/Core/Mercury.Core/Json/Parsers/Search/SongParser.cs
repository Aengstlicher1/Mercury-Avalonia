using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Search
{
    internal static class SongParser
    {
        public static Song Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);

            // Assemble Song
            return new Song()
            {
                Id = IdParser.ParseWatch(renderer),
                Thumbnails = thumbnails,
                Title = FlexColumnParser.Parse(flex, 0)!,
                Artists = FlexColumnParser.Parse(flex, 1, 2)!
            };
        }
    }
}

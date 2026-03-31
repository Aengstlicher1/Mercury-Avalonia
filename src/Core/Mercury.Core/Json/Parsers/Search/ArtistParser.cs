using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Search
{
    internal static class ArtistParser
    {
        internal static Artist Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);

            return new Artist()
            {
                Id = IdParser.ParseBrowse(renderer),
                Title = FlexColumnParser.Parse(flex, 0),
                Audience = FlexColumnParser.Parse(flex, 1, 2),
                Thumbnails = thumbnails,
            };
        }
    }
}

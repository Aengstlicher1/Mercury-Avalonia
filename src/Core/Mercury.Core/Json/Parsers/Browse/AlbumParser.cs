using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Browse
{
    internal static class AlbumParser
    {
        public static Album Parse(JElement renderer, string browseId)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            return new Album()
            {
                Id = browseId,
                Title = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("title"))),
                Year = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("subtitle")), 2),
                Artists = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("straplineTextOne"))),
                Thumbnails = thumbnails
            };
        }
    }
}

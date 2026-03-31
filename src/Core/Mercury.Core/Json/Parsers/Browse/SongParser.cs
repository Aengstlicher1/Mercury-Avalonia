using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Browse
{
    internal static class SongParser
    {
        public static Song Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(renderer);

            return new Song()
            {
                Id = renderer.Get("videoId").AsString().Or(string.Empty),
                Title = renderer.Get("title").Get("runs").GetAt(0).Get("text").AsString().Or(string.Empty),
                Artists = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("longBylineText"))),
                Album = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("longBylineText")), 2),
                Duration = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("lengthText"))),
                Thumbnails = thumbnails,
            };
        }
    }
}

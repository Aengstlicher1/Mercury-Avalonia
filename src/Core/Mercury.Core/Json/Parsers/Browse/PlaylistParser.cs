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
    internal static class PlaylistParser
    {
        public static Playlist Parse(JElement renderer, string browseId)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var runsLength = RunsParser.GetRuns(renderer.Get("secondSubtitle")).Length;

            return new Playlist()
            {
                Id = browseId,
                Title = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("title"))),
                Views = (runsLength > 2) ? string.Empty : RunsParser.Parse(RunsParser.GetRuns(renderer.Get("secondSubtitle"))),
                ItemCount = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("secondSubtitle")), (runsLength > 2) ? 0 : 2),
                Artists = renderer.Get("facepile").Get("avatarStackViewModel").Get("text").Get("content").AsString().Or(string.Empty),
                Thumbnails = thumbnails
            };
        }
    }
}

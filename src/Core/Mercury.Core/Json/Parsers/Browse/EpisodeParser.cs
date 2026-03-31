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
    internal static class EpisodeParser
    {
        public static Episode Parse(JElement nextRenderer, JElement browseRenderer)
        {
            var thumbnails = ThumbnailParser.Parse(nextRenderer);

            return new Episode()
            {
                Id = nextRenderer.Get("videoId").AsString().Or(string.Empty),
                Title = RunsParser.Parse(RunsParser.GetRuns(nextRenderer.Get("title"))),
                PodcastName = RunsParser.Parse(RunsParser.GetRuns(nextRenderer.Get("longBylineText"))),
                Duration = RunsParser.Parse(RunsParser.GetRuns(nextRenderer.Get("lengthText"))),
                Date = RunsParser.Parse(RunsParser.GetRuns(browseRenderer.Get("subtitle"))),
                Thumbnails = thumbnails
            };
        }
    }
}

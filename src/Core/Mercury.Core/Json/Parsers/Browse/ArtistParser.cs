using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Browse
{
    internal static class ArtistParser
    {
        public static Artist Parse(JElement renderer, string browseId)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            return new Artist()
            {
                Id = browseId,
                Title = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("title"))),
                Audience = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("subscriptionButton").Get("subscribeButtonRenderer").Get("subscriberCountText"))),
                Thumbnails = thumbnails
            };
        }
    }
}

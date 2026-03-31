using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Browse
{
    internal static class ProfileParser
    {
        public static Profile Parse(JElement renderer, string browseId)
        {
            var thumbnails = ThumbnailParser.Parse(renderer.Get("foregroundThumbnail").Get("musicThumbnailRenderer"));

            return new Profile()
            {

            };
        }
    }
}

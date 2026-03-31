using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class IdParser
    {
        internal static string ParseBrowse(JElement renderer)
            => renderer
                .Get("navigationEndpoint")
                .Get("browseEndpoint")
                .Get("browseId")
                .AsString()
                .Or(string.Empty);

        internal static string ParseWatch(JElement renderer)
            => renderer
                .Get("flexColumns")
                .GetAt(0)
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .GetAt(0)
                .Get("navigationEndpoint")
                .Get("watchEndpoint")
                .Get("videoId")
                .AsString()
                .Or(string.Empty);
    }
}

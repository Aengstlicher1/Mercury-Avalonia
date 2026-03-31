using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class FixedColumnParser
    {
        public static string Parse(JArray fix, int columnIndex, int runIndex = 0)
        {
            return fix[columnIndex]
                .Get("musicResponsiveListItemFixedColumnRenderer")
                .Get("text")
                .Get("runs")
                .GetAt(runIndex)
                .Get("text")
                .AsString()
                .Or(string.Empty);
        }

        public static JArray GetFix(JElement parent)
        {
            return parent
                .Get("fixedColumns")
                .AsArray()
                .Or(JArray.Empty);
        }
    }
}

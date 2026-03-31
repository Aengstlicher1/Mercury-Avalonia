using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class FlexColumnParser
    {
        internal static string Parse(JArray flex, int colIndex, int runIndex = 0)
        {
            var runs = flex[colIndex]
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .AsArray()
                .Or(JArray.Empty);

            var text = RunsParser.Parse(runs, runIndex);

            if (text == null) Debug.WriteLine($"Unable to parse FlexColumn: {flex}", "PARSERS");
            return text!;
        }

        internal static JArray GetFlex(JElement renderer)
            =>  renderer
                .Get("flexColumns")
                .AsArray()
                .Or(JArray.Empty);
    }
}

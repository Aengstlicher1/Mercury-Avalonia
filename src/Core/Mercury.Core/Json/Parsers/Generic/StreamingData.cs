using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class StreamingDataParser
    {
        public static StreamingData Parse(JElement json)
        {
            var expiresIn = TimeSpan.FromSeconds(int.Parse(json
                .Get("streamingData")
                .Get("expiresInSeconds")
                .AsString()
                .Or(string.Empty)));

            var details = json
                .Get("videoDetails");

            return new StreamingData()
            {
                ExpiresAt = DateTime.Now + expiresIn,
                Id = details.Get("videoId").AsString().Or(string.Empty),
                Title = details.Get("title").AsString().Or(string.Empty),
                Duration = TimeSpan.FromSeconds(int.Parse(details.Get("lengthSeconds").AsString().Or(string.Empty))),
                ViewCount = int.Parse(details.Get("viewCount").AsString().Or(string.Empty))
            };
        }
    }
}

using Mercury.Core.Models;
using Mercury.Core.Utils;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;


namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class ThumbnailParser
    {
        internal static ThumbArray Parse(JElement thumbRenderer)
        {
            var thumbs = thumbRenderer
                .Get("thumbnail")
                .Get("thumbnails")
                .AsArray()
                .Or(JArray.Empty);

            if (thumbs != null && thumbs.Length > 0)
            {
                Collection<Thumbnail> thumbnails = new ();

                for (int i = 0; i < thumbs.Length; i++)
                {
                    thumbnails.Add(
                        new Thumbnail()
                        {
                            Url = thumbs[i].Get("url").AsString().Or(string.Empty),
                            Size = new Dimensions()
                            {
                                Width = thumbs[i].Get("width").AsInt32().Or(0),
                                Height = thumbs[i].Get("height").AsInt32().Or(0)
                            }
                        }
                    );
                }

                if (thumbnails.First().TryGetCustomSize(new Dimensions(1080), out Thumbnail highRes))
                    thumbnails.Add(highRes);

                return new ThumbArray(thumbnails.ToArray());
            }
            else
                return ThumbArray.Empty;
        }

        public static JElement GetThumbRenderer(JElement renderer)
        {
            return renderer
                .Get("thumbnail")
                .Get("musicThumbnailRenderer");
        }
    }
}

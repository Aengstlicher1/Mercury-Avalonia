using Mercury.Core.Json.Parsers.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mercury.Core.Models
{
    public struct ThumbArray : IEnumerable
    {
        public ThumbArray(Thumbnail[] thumbnails) 
        {
            Thumbs = thumbnails;
        }

        private Thumbnail[] Thumbs { get; set; }

        public Thumbnail HighestRes => Thumbs.MaxBy(t => t.Size.Height);
        public Thumbnail LowestRes => Thumbs.MinBy(t => t.Size.Height);

        public IEnumerator GetEnumerator()
            => Thumbs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static ThumbArray Empty { get => new ThumbArray(); }
    }

    public struct Thumbnail
    {
        public Thumbnail(string url, Dimensions size)
        {
            Url = url;
            Size = size;
        }
        public Thumbnail() { }

        public string Url { get; set; } = "";
        public Dimensions Size { get; set; }

        public override string ToString()
        {
            return $"{Size} - {Url.Substring(0, 24)}...";
        }

        public bool TryGetCustomSize(Dimensions size, out Thumbnail thumbnail)
        {
            thumbnail = new Thumbnail();

            var isAdjustable = isAdjustableUrl(Url);
            if (isAdjustable)
            {
                var customResUrl = Regex.Replace(Url, @"=w\d+-h\d+-l\d+", $"=w{size.Width}-h{size.Height}-l90");
                thumbnail = new Thumbnail()
                {
                    Url = customResUrl,
                    Size = size
                };
            }
            return isAdjustable;
        }

        private static bool isAdjustableUrl(string url)
        {
            var regex = new Regex(@"=w\d+-h\d+-l\d+");
            var match = regex.Match(url);

            return match.Success;
        }
    }

    public struct Dimensions
    {
        public Dimensions (int height, int width)
        {
            Height = height;
            Width = width;
        }
        public Dimensions (int size)
        {
            Height = size;
            Width = size;
        }

        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}

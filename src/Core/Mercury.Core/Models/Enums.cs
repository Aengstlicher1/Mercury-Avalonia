using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Models
{
    public static class Enums
    {
        public enum MediaCategory
        {
            None,
            Song,
            Video,
            Playlist,
            Album,
            Artist,
            Profile,
            Podcast,
            Episode
        }

        public enum SearchFilter
        {
            All,
            Songs,
            Videos,
            Albums,
            Artists,
            CommunityPlaylists,
            FeaturedPlaylists,
            Podcasts,
            Episodes,
            Profiles
        }

        public static string ToParam(this SearchFilter category, bool ignoreSpelling = true)
        {
            const string filteredQueryParam1 = "EgWKAQ";
            string queryParam1 = "";
            string queryParam2 = "";
            string queryParam3 = "";


            if (category == SearchFilter.CommunityPlaylists || category == SearchFilter.FeaturedPlaylists)
            {
                queryParam1 = "EgeKAQQoA";
                queryParam3 = ignoreSpelling ? "BQgIIAWoMEA4QChADEAQQCRAF" : "BagwQDhAKEAMQBBAJEAU%3D";
            }
            else
            {
                queryParam1 = filteredQueryParam1;
                queryParam3 = ignoreSpelling ? "AUICCAFqDBAOEAoQAxAEEAkQBQ%3D%3D" : "AWoMEA4QChADEAQQCRAF";
            }
            queryParam2 = category switch
            {
                SearchFilter.Songs => "II",
                SearchFilter.Videos => "IQ",
                SearchFilter.Albums => "IY",
                SearchFilter.Artists => "Ig",
                SearchFilter.CommunityPlaylists => "EA",
                SearchFilter.FeaturedPlaylists => "Dg",
                SearchFilter.Profiles => "JY",
                SearchFilter.Podcasts => "JQ",
                SearchFilter.Episodes => "JI",
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, "The given SearchFilter is invalid.")
            };

            return queryParam1 + queryParam2 + queryParam3;
        }
    }
}

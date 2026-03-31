using Mercury.Core.Services;

namespace Mercury.Core
{
    public static class YoutubeMusic
    {
        public static SearchService Search = new SearchService();

        public static PlayerService Player = new PlayerService();

        public static LyricsService Lyrics = new LyricsService();

        public static BrowseService Browse = new BrowseService();
    }
}

using Mercury.Core.Models;
using static Mercury.Core.Models.Enums;


namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var medias = await YoutubeMusic.Search.SearchCategoryAsync("test", SearchFilter.CommunityPlaylists);

            var media = medias!.First();

            var test = await YoutubeMusic.Browse.GetInfoAsync("PL2EWRRvBchexFAg4Ow4e7oQDrbqxxdhqY", MediaCategory.Playlist);
        }
    }
}
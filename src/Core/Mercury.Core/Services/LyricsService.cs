using Mercury.Core.Models;
using Mercury.Core.Network;
using System.Text.Json;


namespace Mercury.Core.Services
{
    public sealed class LyricsService
    {
        private const string BaseUrl = "https://lrclib.net/api";

        public async Task<LyricsResult?> GetLyricsAsync(Track track)
        {
            try
            {
                var url = $"{BaseUrl}/get?artist_name={Uri.EscapeDataString(track.Artists)}&track_name={Uri.EscapeDataString(track.Title)}";

                if (track.DurationTimeSpan.TotalSeconds > 0)
                    url += $"&duration={Convert.ToInt32(track.DurationTimeSpan.TotalSeconds)}";

                var response = await RequestHandler.httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<LyricsResult>(jsonString);
            }
            catch (HttpRequestException ex)
            {
                return new LyricsResult() { PlainLyrics = "Http Error: " + ex.Message };
            }
        }
    }
}

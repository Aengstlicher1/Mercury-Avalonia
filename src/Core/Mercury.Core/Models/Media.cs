using static Mercury.Core.Models.Enums;

namespace Mercury.Core.Models;

/// <summary>Base class for all search results</summary>
public abstract class Media
{
    public virtual string Id { get; set; } = "";
    public ThumbArray Thumbnails { get; set; } = ThumbArray.Empty;
    public virtual string Title { get; init; } = "";
    public virtual string Artists { get; init; } = "";
    public virtual MediaCategory Type { get; } = MediaCategory.None;
}

public abstract class Track : Media
{
    public virtual string Duration { get; set; } = "";

    public TimeSpan DurationTimeSpan
    {
        get
        {
            if (string.IsNullOrEmpty(Duration)) 
                return TimeSpan.Zero;

            var parts = Duration.Split(':');
            return parts.Length switch
            {
                2 => new TimeSpan(0, int.Parse(parts[0]), int.Parse(parts[1])),
                3 => new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])),
                _ => TimeSpan.Zero
            };
        }
    }
}

/// <summary>Song search result with duration</summary>
public class Song : Track
{
    public override MediaCategory Type { get; } = MediaCategory.Song;

    public string Album { get; set; } = "";

    public string Url => $"https://music.youtube.com/watch?v={Id}";
    
    
}

/// <summary>Video search result</summary>
public class Video : Track
{
    public override MediaCategory Type { get; } = MediaCategory.Video;

    public string Views { get; set; } = "";

    public string Url => $"https://music.youtube.com/watch?v={Id}";
}

/// <summary>Album search result</summary>
public class Album : Media
{
    public override MediaCategory Type { get; } = MediaCategory.Album;

    public string Year { get; set; } = "";
    
    public string Url => $"https://music.youtube.com/browse/{Id}";
}

/// <summary>Artist search result</summary>
public class Artist : Media
{
    public override MediaCategory Type { get; } = MediaCategory.Artist;

    public string Audience { get; set; } = "";
    
    public string Url => $"https://music.youtube.com/channel/{Id}";
}

/// <summary>Playlist search result</summary>
public class Playlist : Media
{
    public override MediaCategory Type { get; } = MediaCategory.Playlist;

    public string Views { get; set; } = "";

    public string ItemCount { get; set; } = "";
    
    public string Url => $"https://music.youtube.com/playlist?list={Id}";
}

/// <summary>Podcast search result</summary>
public class Podcast : Media
{
    public override MediaCategory Type { get; } = MediaCategory.Podcast;

    public string Url => $"https://music.youtube.com/podcast/{Id}";
}

/// <summary>Episode search result</summary>
public class Episode : Track
{
    public override MediaCategory Type { get; } = MediaCategory.Episode;

    public string PodcastName { get; set; } = "";
    public string Date { get; set; } = "";
    
    public string Url => $"https://music.youtube.com/watch?v={Id}";
}

public class Profile : Media
{
    public override MediaCategory Type { get; } = MediaCategory.Profile;

    public string Tag { get; set; } = "";

}

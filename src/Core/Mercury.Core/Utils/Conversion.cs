using System.Globalization;
using Mercury.Core.Network;
using static Mercury.Core.Models.Enums;

namespace Mercury.Core.Utils;

/// <summary>
/// Contains extension methods for converting data types.
/// </summary>
public static class Conversion
{
    /// <summary>
    /// Converts a <see cref="ClientType"/> to a <see cref="Client"/>.
    /// </summary>
    /// <param name="type">The client type.</param>
    /// <returns>A <see cref="Client"/> representing the type.</returns>
    /// <exception cref="ArgumentException">Occurs when an invalid client type is passed.</exception>
    internal static Client? ToClient(
        this ClientType type) =>
        type switch
        {
            ClientType.None => null,
            ClientType.WebMusic => Client.WebMusic.Clone(),
            ClientType.IOSMusic => Client.IOSMusic.Clone(),
            ClientType.Web => Client.Web.Clone(),
            ClientType.Android => Client.Android.Clone(),
            ClientType.AndroidVR => Client.AndroidVR.Clone(),
            _ => throw new ArgumentException($"Invalid client type: {type}.", nameof(type))
        };


    /// <summary>
    /// Converts a <see langword="string"/> to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>A <see cref="TimeSpan"/> representing the text.</returns>
    public static TimeSpan? ToTimeSpan(
        this string? text)
    {
        TimeSpan? ParseShort()
        {
            string[] parts = text.Split(':');
            return parts.Length switch
            {
                2 when int.TryParse(parts[0], out int minutes) && int.TryParse(parts[1], out int seconds) => new(0, minutes, seconds),
                3 when int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes) && int.TryParse(parts[2], out int seconds) => new(hours, minutes, seconds),
                _ => null,
            };
        }

        TimeSpan? ParseLong()
        {
            string[] parts = text.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length % 2 != 0)
                return null;

            int hours = 0, minutes = 0, seconds = 0;
            for (int i = 0; i < parts.Length; i += 2)
            {
                if (!int.TryParse(parts[i], out int value))
                    return null;

                string unit = parts[i + 1];
                switch (unit)
                {
                    case "hr":
                    case "hrs":
                    case "hour":
                    case "hours":
                        hours = value;
                        break;

                    case "min":
                    case "mins":
                    case "minute":
                    case "minutes":
                        minutes = value;
                        break;

                    case "sec":
                    case "secs":
                    case "second":
                    case "seconds":
                        seconds = value;
                        break;

                    default:
                        return null;
                }
            }

            return new(hours, minutes, seconds);
        }


        if (text is null)
            return null;

        if (text.Contains(':'))
            return ParseShort();

        return ParseLong();
    }

    /// <summary>
    /// Converts a milliseconds <see langword="long"/> to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="milliseconds">The milliseconds to convert.</param>
    /// <returns>A <see cref="TimeSpan"/> representing the milliseconds.</returns>
    public static TimeSpan? ToTimeSpan(
        this long? milliseconds)
    {
        if (milliseconds.HasValue)
            return TimeSpan.FromMilliseconds(milliseconds.Value);

        return null;
    }


    /// <summary>
    /// Converts a <see langword="string"/> to a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>A  <see cref="DateTime"/> representing the text.</returns>
    public static DateTime? ToDateTime(
        this string? text)
    {
        if (text is null)
            return null;

        if (!text.Contains("ago") && DateTime.TryParse(text, CultureInfo.InvariantCulture, out DateTime result))
            return result;

        text = text.Replace(" ago", "").Trim();

        int i = 0;
        while (i < text.Length && char.IsDigit(text[i]))
            i++;

        if (i == 0)
            return null;

        int timeSpanValue = int.Parse(text.Substring(0, i));
        string timeSpanKind = text.Substring(i).Trim();

        return timeSpanKind[0] switch
        {
            'd' => DateTime.Now - TimeSpan.FromDays(timeSpanValue),
            'h' => DateTime.Now - TimeSpan.FromHours(timeSpanValue),
            'm' => DateTime.Now - TimeSpan.FromMinutes(timeSpanValue),
            's' => DateTime.Now - TimeSpan.FromSeconds(timeSpanValue),
            _ => null
        };
    }


    /// <summary>
    /// Converts a <see langword="string"/> to an <see cref="int"/>.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>An <see cref="int"/> representing the text.</returns>
    public static int? ToInt32(
        this string? text)
    {
        if (int.TryParse(text, CultureInfo.InvariantCulture, out int result))
            return result;

        return null;
    }

    /// <summary>
    /// Converts a <see langword="string"/> to a <see cref="long"/>.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>A <see cref="long"/> representing the text.</returns>
    public static long? ToInt64(
        this string? text)
    {
        if (long.TryParse(text, CultureInfo.InvariantCulture, out long result))
            return result;

        return null;
    }

    public static SearchFilter ToFilter(this MediaCategory category)
    {
        return category switch
        {
            MediaCategory.Song      => SearchFilter.Songs,
            MediaCategory.Video     => SearchFilter.Videos,
            MediaCategory.Playlist  => SearchFilter.CommunityPlaylists,
            MediaCategory.Album     => SearchFilter.Albums,
            MediaCategory.Artist    => SearchFilter.Artists,
            MediaCategory.Podcast  => SearchFilter.Podcasts,
            MediaCategory.Episode   => SearchFilter.Episodes,
            _                       => SearchFilter.Songs
        };
    }

    public static MediaCategory ToCategory(this SearchFilter category)
    {
        return category switch
        {
            SearchFilter.Songs                  => MediaCategory.Song,
            SearchFilter.Videos                 => MediaCategory.Video,
            SearchFilter.CommunityPlaylists     => MediaCategory.Playlist,
            SearchFilter.FeaturedPlaylists      => MediaCategory.Playlist,
            SearchFilter.Albums                 => MediaCategory.Album,
            SearchFilter.Artists                => MediaCategory.Artist,
            SearchFilter.Podcasts               => MediaCategory.Podcast,
            SearchFilter.Episodes               => MediaCategory.Episode,
            _                                   => MediaCategory.None
        };
    }
}
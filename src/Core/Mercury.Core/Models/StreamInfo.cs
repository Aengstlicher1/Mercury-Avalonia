namespace Mercury.Core.Models
{
    public abstract class StreamInfo
    {
        public int ITag { get; init; }
        public string Url { get; init; } = "";
        public string MimeType { get; init; } = "";
        public int Bitrate { get; init; }
        public string Quality { get; init; } = "";
    }

    public sealed class MuxedStreamInfo : StreamInfo
    {
        public Dimensions Size { get; init; }
        public int Fps { get; init; }
        public string QualityLabel { get; init; } = "";
        public string AudioQuality { get; init; } = "";
    }

    public sealed class VideoStreamInfo : StreamInfo
    {
        public Dimensions Size { get; init; }
        public int Fps { get; init; }
        public string QualityLabel { get; init; } = "";
        public int AverageBitrate { get; init; }
    }

    public sealed class AudioStreamInfo : StreamInfo
    {
        public string AudioQuality { get; init; } = "";
        public int AudioSampleRate { get; init; }
        public int AverageBitrate { get; init; }
    }
}

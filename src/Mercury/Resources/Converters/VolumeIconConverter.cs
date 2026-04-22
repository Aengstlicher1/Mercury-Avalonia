using System;
using System.Globalization;
using Avalonia.Data.Converters;
using IconPacks.Avalonia.MaterialDesign;

namespace Mercury.Resources.Converters;

public class VolumeIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int volume)
        {
            switch (volume)
            {
                case 0:
                    return PackIconMaterialDesignKind.VolumeOffRound;
                case > 0 and <= 33:
                    return PackIconMaterialDesignKind.VolumeMuteRound;
                case > 33 and <= 66:
                    return PackIconMaterialDesignKind.VolumeDownRound;
                case > 66 and <= 100:
                    return PackIconMaterialDesignKind.VolumeUpRound;
                default:
                    return PackIconMaterialDesignKind.VolumeOffRound; // fallback
            }
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}
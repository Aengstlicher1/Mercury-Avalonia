using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

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
                    return MaterialIconKind.VolumeMute;
                case > 0 and <= 33:
                    return MaterialIconKind.VolumeLow;
                case > 33 and <= 66:
                    return MaterialIconKind.VolumeMedium;
                case > 66 and <= 100:
                    return MaterialIconKind.VolumeHigh;
                default:
                    return MaterialIconKind.VolumeMedium; // fallback
            }
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
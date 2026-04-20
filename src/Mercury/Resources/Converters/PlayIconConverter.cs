using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

namespace Mercury.Resources.Converters;

public class PlayIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool playing)
        {
            switch (playing)
            {
                case true: return MaterialIconKind.Pause;
                case false: return MaterialIconKind.Play;
            }
        }
        return MaterialIconKind.PlayPause;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}
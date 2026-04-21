using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Mercury.Resources.Converters;

public class ColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return _convert(value);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return _convert(value);
    }

    private object? _convert(object? value)
    {
        if (value != null && value is System.Drawing.Color sysColor)
        {
            return Avalonia.Media.Color.FromArgb(
                sysColor.A,
                sysColor.R,
                sysColor.G,
                sysColor.B
            );
        }
        else if (value != null && value is Avalonia.Media.Color avaColor)
        {
            return System.Drawing.Color.FromArgb(
                avaColor.A,
                avaColor.R,
                avaColor.G,
                avaColor.B
            );
        }

        return null;
    }
}
using System;
using System.Globalization;
using Avalonia.Data.Converters;
using IconPacks.Avalonia.MaterialDesign;

namespace Mercury.Resources.Converters;

public class PlayIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool playing)
        {
            switch (playing)
            {
                case true: return PackIconMaterialDesignKind.PauseRound;
                case false: return PackIconMaterialDesignKind.PlayArrowRound;
            }
        }
        return PackIconMaterialDesignKind.PlayDisabledRound;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}
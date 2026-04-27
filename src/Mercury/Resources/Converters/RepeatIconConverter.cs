using System;
using System.Globalization;
using Avalonia.Data.Converters;
using IconPacks.Avalonia.MaterialDesign;
using Mercury.Models;

namespace Mercury.Resources.Converters;

public class RepeatIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RepeatState state)
        {
            switch (state)
            {
                case RepeatState.NoRepeat:
                    return PackIconMaterialDesignKind.SyncDisabledRound;
                case RepeatState.RepeatSingle:
                    return PackIconMaterialDesignKind.RepeatOneRound;
                case RepeatState.RepeatAll:
                    return PackIconMaterialDesignKind.RepeatRound;
                case  RepeatState.Shuffle:
                    return PackIconMaterialDesignKind.ShuffleRound;
                    
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}
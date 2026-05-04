using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace Mercury.Resources.Converters;

public class CollectionNotAnyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            ICollection collection => collection.Count == 0,
            IEnumerable enumerable => !enumerable.Cast<object?>().Any(),
            _ => throw new ArgumentException($"{nameof(value)} is not a Collection")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Mercury.Resources.Converters;

public class CollectionIndexConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[0] is null || values[1] is not IList collection)
            return null;

        var index = collection.IndexOf(values[0]);
        return index >= 0 ? (index + 1).ToString() : null;
    }
}
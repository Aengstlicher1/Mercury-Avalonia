using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Mercury.Core.Models;

namespace Mercury.Resources.Converters;

public class TracksEqualConverter : IMultiValueConverter
{
    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values != null)
        {
            string? lastId = null;
            
            foreach (var value in values)
            {
                if (value is not Track track)
                    return false;
                
                lastId ??= track.Id;
                if (lastId != track.Id)
                    return false;
            }

            return true;
        }
        
        throw new ArgumentNullException(nameof(values));
    }
}
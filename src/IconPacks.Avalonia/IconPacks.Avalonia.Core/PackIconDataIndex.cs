#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace IconPacks.Avalonia.Core;

public static class PackIconDataIndex
{
    private static readonly object Gate = new();
    private static readonly Dictionary<Type, ReadOnlyDictionary<Enum, string>> Packs = new();

    public static ReadOnlyDictionary<Enum, string> Register(Type enumType, IDictionary<Enum, string>? icons = null)
    {
        lock (Gate)
        {
            if (!Packs.TryGetValue(enumType, out ReadOnlyDictionary<Enum, string>? value))
            {
                // PackIconDataFactory<TEnum>.Create() is generic — close it over enumType via reflection
                IDictionary src;
                if (icons is not null)
                {
                    src = (IDictionary)icons;
                }
                else
                {
                    var factoryType = typeof(PackIconDataFactory<>).MakeGenericType(enumType);
                    var createMethod = factoryType.GetMethod(
                                           "Create",
                                           BindingFlags.Public | BindingFlags.Static)
                                       ?? throw new InvalidOperationException(
                                           $"PackIconDataFactory<{enumType.Name}>.Create not found.");
                    src = (IDictionary)createMethod.Invoke(null, null)!;
                }

                var dst = new Dictionary<Enum, string>(src.Count);
                foreach (DictionaryEntry kv in src)
                {
                    dst[(Enum)kv.Key] = (string)kv.Value!;
                }

                value = new ReadOnlyDictionary<Enum, string>(dst);
                Packs[enumType] = value;
            }
            return value;
        }
    }

    public static bool TryGetPath(Enum kind, out string? path)
    {
        var type = kind.GetType();
        ReadOnlyDictionary<Enum, string>? packDictionary;
        lock (Gate)
        {
            if (!Packs.TryGetValue(type, out packDictionary))
            {
                packDictionary = Register(kind.GetType());
            }
        }
        
        return packDictionary.TryGetValue(kind, out path);
    }
}
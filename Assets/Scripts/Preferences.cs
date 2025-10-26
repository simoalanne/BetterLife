using System;
using UnityEngine;

/// <summary> Contains all player preferences used by the game. </summary>
public static class Preferences
{
    public static float Volume
    {
        get => Get(nameof(Volume), 1f);
        set => Set(nameof(Volume), value);
    }

    public static bool RouletteShowBetType
    {
        get => Get(nameof(RouletteShowBetType), true);
        set => Set(nameof(RouletteShowBetType), value);
    }

    public static bool RouletteShowBetOdds
    {
        get => Get(nameof(RouletteShowBetOdds), true);
        set => Set(nameof(RouletteShowBetOdds), value);
    }
    
    private static T Get<T>(string key, T defaultValue) => typeof(T) switch
    {
        _ when typeof(T) == typeof(int) => (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue),
        _ when typeof(T) == typeof(float) => (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue),
        _ when typeof(T) == typeof(string) => (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue),
        _ when typeof(T) == typeof(bool) =>
            (T)(object)(PlayerPrefs.GetInt(key, (bool)(object)defaultValue ? 1 : 0) == 1),
        _ => throw new NotSupportedException($"Type {typeof(T)} is not supported by Preferences.Get")
    };

    private static void Set<T>(string key, T value)
    {
        var objValue = (object)value;
        Action setAction = typeof(T) switch
        {
            _ when typeof(T) == typeof(int) => () => PlayerPrefs.SetInt(key, (int)objValue),
            _ when typeof(T) == typeof(float) => () => PlayerPrefs.SetFloat(key, (float)objValue),
            _ when typeof(T) == typeof(string) => () => PlayerPrefs.SetString(key, (string)objValue),
            _ when typeof(T) == typeof(bool) => () => PlayerPrefs.SetInt(key, (bool)objValue ? 1 : 0),
            _ => throw new NotSupportedException($"Type {typeof(T)} is not supported by Preferences.Set")
        };
        setAction.Invoke();
    }
}

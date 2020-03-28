using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class Settings
{
    public static string resolutionCurrent = PlayerPrefs.GetString("resolution", $"{Screen.width}x{Screen.height}");
    public static string resolutionSave = resolutionCurrent;
    public static string[] resolutions = FetchResolutions();

    public static string screenmodeCurrent = PlayerPrefs.GetString("mode", "Full screen");
    public static string screenmodeSave = screenmodeCurrent;
    public static string[] screenmodes = new string[] { "Full screen", "Windowed" };

    public static string lightingCurrent = PlayerPrefs.GetString("lighting", "High quality");
    public static string lightingSave = lightingCurrent;
    public static string[] lightings = new string[] { "High quality", "Medium quality", "Low quality"};

    public static FieldInfo[] currents = Array.FindAll(typeof(Settings).GetFields(), field => field.Name.EndsWith("Current"));
    public static FieldInfo[] saves = Array.FindAll(typeof(Settings).GetFields(), field => field.Name.EndsWith("Save"));

    private static string[] FetchResolutions()
    {
        var resolutions = Screen.resolutions;
        var accepted = new List<string>();
        foreach (var res in resolutions)
        {
            float multiplier = (float)res.width / 16f;
            bool rightResolution = Mathf.RoundToInt(multiplier * 9f) == res.height;
            var a = accepted.Any(ac => ac == $"{res.width}x{res.height}");
            if (rightResolution && !a) accepted.Add($"{res.width}x{res.height}");
            if ($"{res.width}x{res.height}" == resolutionCurrent)
            {
                Debug.Log($"Current resolution found at {resolutionCurrent}");
            }
        }
        accepted.Reverse();
        return accepted.ToArray();
    }
    public static void SetResolution(int width, int height, FullScreenMode mode)
    {
        Screen.SetResolution(width, height, mode);
        PlayerPrefs.SetString("resolution", $"{width}x{height}");
    }
    public static Vector2Int ResolutionToVector
    {
        get
        {
            var a = Settings.resolutionCurrent.IndexOf("x");
            var width = System.Int32.Parse(Settings.resolutionCurrent.Substring(0, a));
            var height = System.Int32.Parse(Settings.resolutionCurrent.Substring(a + 1, Settings.resolutionCurrent.Length - a - 1));
            return new Vector2Int(width, height);
        }
    }
    public static void RestoreValues()
    {
        for (int i = 0; i < saves.Length; i++)
        {
            saves[i].SetValue(null, currents[i].GetValue(null));
        }
    }
}

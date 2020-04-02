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
    public static string[] lightings = new string[] { "Low quality", "Medium quality", "High quality"};

    public static string mastervolumeCurrent = PlayerPrefs.GetString("mastervolume", "5");
    public static string mastervolumeSave = mastervolumeCurrent;

    public static string sfxvolumeCurrent = PlayerPrefs.GetString("sfxvolume", "7");
    public static string sfxvolumeSave = mastervolumeCurrent;

    public static string musicvolumeCurrent = PlayerPrefs.GetString("musicvolume", "1");
    public static string musicvolumeSave = mastervolumeCurrent;

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
        }
        return accepted.ToArray();
    }

    public static void SetResolution(Vector2Int newRes, FullScreenMode mode)
    {
        Screen.SetResolution(newRes.x, newRes.y, mode);
        PlayerPrefs.SetString("resolution", $"{newRes.x}x{newRes.y}");
    }

    public static FullScreenMode StringToScreenMode(string mode)
    {
        PlayerPrefs.SetString("mode", mode);
        switch (mode)
        {
            case "Full screen":
                return FullScreenMode.ExclusiveFullScreen;
            case "Windowed":
                return FullScreenMode.Windowed;
        }
        return FullScreenMode.ExclusiveFullScreen;
    }

    public static Vector2Int ResolutionToVector(string res)
    {
            var a = res.IndexOf("x");
            var width = System.Int32.Parse(res.Substring(0, a));
            var height = System.Int32.Parse(res.Substring(a + 1, res.Length - a - 1));
            return new Vector2Int(width, height);
    }

    public static void RestoreValues()
    {
        for (int i = 0; i < saves.Length; i++)
        {
            saves[i].SetValue(null, currents[i].GetValue(null));
        }
    }

    public static void SaveSettings()
    {
        if (resolutionCurrent != resolutionSave || screenmodeSave != screenmodeCurrent)
        {
            screenmodeCurrent = screenmodeSave;
            FullScreenMode mode = StringToScreenMode(screenmodeCurrent);
            Debug.Log("Saving resolution at " + resolutionSave);
            SetResolution(ResolutionToVector(resolutionSave), mode);
            resolutionCurrent = resolutionSave;
        }
        for (int i = 0; i < saves.Length; i++)
        {
            if (saves[i].Name.Contains("volume"))
            {
                var save = saves[i].GetValue(null);
                var current = currents[i].GetValue(null);
                if (save != current)
                {
                    var name = saves[i].Name.Substring(0, saves[i].Name.Length - 4);
                    currents[i].SetValue(null, save);
                    PlayerPrefs.SetString(name, save.ToString());
                    Audio.instance.VolumeReset(name, Int32.Parse(save.ToString()));
                }
            }
        }
    }
}

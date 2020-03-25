using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Settings
{
    public static int screenHeight = PlayerPrefs.GetInt("height", Screen.currentResolution.height);
    public static int screenWidth = PlayerPrefs.GetInt("width", Screen.currentResolution.width);

    public static Resolution[] resolution = FetchResolutions();
    public static int resolutionCurrent;

    public static string fullScreenMode = PlayerPrefs.GetString("mode", FullScreenMode.ExclusiveFullScreen.ToString());
     
    private static Resolution[] FetchResolutions()
    {
        Debug.Log($"{screenWidth}x{screenHeight}");
        var resolutions = Screen.resolutions;
        List<Resolution> accepted = new List<Resolution>();
        foreach (var res in resolutions)
        {
            float multiplier = (float)res.width / 16f;
            bool rightResolution = Mathf.RoundToInt(multiplier * 9f) == res.height;
            var a = accepted.Any(ac => ac.width == res.width);
            if (rightResolution && !a) accepted.Add(res);
            if (res.width == screenWidth && res.height == screenHeight)
            {
                Debug.Log($"Current resolution found at {screenWidth}x{screenHeight}");
                resolutionCurrent = accepted.Count - 1;
            }
        }
        accepted.Reverse();
        return accepted.ToArray();
    }
    public static void SetResolution(int width, int height, FullScreenMode mode)
    {
        Screen.SetResolution(width, height, mode);
    }
}

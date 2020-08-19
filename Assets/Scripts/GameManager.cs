using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject();
                _instance = go.AddComponent<GameManager>();
            };
            return _instance;
        }
    }

    [HideInInspector]
    public float time = 0f;
    [HideInInspector]
    public TextMeshProUGUI timer;

    private void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        UpdateTime();
    }
    private void UpdateTime()
    {
        time += Time.deltaTime;
        timer.text = $"{(int)time / 60:0}:{time % 60:00.0}";
    }
}

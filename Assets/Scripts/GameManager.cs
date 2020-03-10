using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance {
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
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        UpdateTime();
    }
    private void UpdateTime()
    {
        time += Time.deltaTime;
        float minutes = Mathf.Floor(time / 60);
        float seconds = Mathf.Floor(time - minutes * 60);
        float milliseconds = Mathf.Floor((time % 1) * 10f);
        timer.text = string.Format("{0:0}:{1:00}.{2:0}", minutes, seconds, milliseconds);
    }
}

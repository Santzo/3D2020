using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        GameManager.instance.name = "GameManager";
        Audio.instance.name = "AudioManager";
    }
}

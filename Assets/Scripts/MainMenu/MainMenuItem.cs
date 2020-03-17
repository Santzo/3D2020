using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuItem : MonoBehaviour
{
    internal Animator anim;
    internal TextMeshProUGUI text;
    internal int selected;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();
        selected = Animator.StringToHash("Selected");
    }
}

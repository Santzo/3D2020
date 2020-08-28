using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Meat : MonoBehaviour
{
    MeshRenderer rend;
    VisualEffect effect;
    BoxCollider collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        rend = GetComponent<MeshRenderer>();
        effect = GetComponent<VisualEffect>();
        effect.Stop();
    }

    public void PickUp()
    {
        collider.enabled = false;
        rend.enabled = false;
        effect.Play();
        Destroy(gameObject, 2f);
    }
}

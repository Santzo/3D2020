using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{

    private void Start()
    {
        foreach (Transform trans in transform)
        {
            // var col = trans.GetComponent<Collider>();
            // if (col == null)
            // {
            //     var collider = trans.gameObject.AddComponent<BoxCollider2D>();
            // }
        }
    }
}

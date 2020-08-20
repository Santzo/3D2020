using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform trans in transform)
        {
            if (trans.name.StartsWith("Pine"))
            {
                var mat = trans.GetChild(0).GetComponent<MeshRenderer>().material;
                mat.SetFloat("Power", Random.Range(-0.15f, 0.15f));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpear : MonoBehaviour
{
    [SerializeField]
    float waitDelay, multiplier = 1f;
    private void Awake()
    {
        GetComponent<Animator>().SetFloat("Multiplier", multiplier);
        if (waitDelay > 0f)
        {
            GetComponent<Animator>().enabled = false;
            StartCoroutine(Wait());
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitDelay);
        GetComponent<Animator>().enabled = true;
    }


}

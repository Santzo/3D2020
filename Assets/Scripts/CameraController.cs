using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform middlePoint;
    Transform player;

    private void Start()
    {
        middlePoint = GameObject.Find("MiddlePoint").transform;
        player = GameObject.Find("Player").transform;
    }
    private void Update()
    {
        Vector3 direction = -(middlePoint.position - player.position) * 1.4f;
        transform.position = new Vector3(direction.x, player.position.y + 9.5f, direction.z);
        
        transform.LookAt(player, Vector2.up);
    }

}

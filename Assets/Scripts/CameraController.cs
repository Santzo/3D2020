using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform middlePoint;
    Transform player;
    Vector3 vel = Vector3.zero;

    private void Start()
    {
        middlePoint = GameObject.Find("MiddlePoint").transform;
        player = GameObject.Find("Player").transform;
    }
    private void LateUpdate()
    {
        Vector3 direction = -(middlePoint.position - player.position) * 1.4f;
        Vector3 pos = new Vector3(direction.x, player.position.y + 9.5f, direction.z);
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref vel, 0.15f);
        transform.LookAt(player, Vector2.up);
    }

}

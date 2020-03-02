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
        float posX = Mathf.Clamp(direction.x, -70f, 70f);
        float posZ = Mathf.Clamp(direction.z, -70f, 70f);
        Vector3 pos = new Vector3(posX, player.position.y + 7.5f, posZ);
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref vel, 0.15f);
        transform.LookAt(player, Vector2.up);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform middlePoint;
    Transform player;
    Vector3 vel = Vector3.zero;
    [SerializeField] float distance = 1f, height = 10f;

    private void Start()
    {
        middlePoint = GameObject.Find("MiddlePoint").transform;
        player = GameObject.Find("Player").transform;
    }
    private void LateUpdate()
    {
        Vector3 direction = -(middlePoint.position - player.position);
        Vector3 point = middlePoint.position + direction * distance + (Vector3.up * height);
        point = new Vector3(Mathf.Clamp(point.x, -100f, 100f), Mathf.Clamp(point.y, -100f, 100f), Mathf.Clamp(point.z, -100f, 100f));
        transform.position = Vector3.SmoothDamp(transform.position, point, ref vel, 0.15f);
        transform.LookAt(player, Vector2.up);
    }

}

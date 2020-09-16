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
        transform.position = CalculatePoint();
        transform.LookAt(player, Vector3.up);
    }
    private void LateUpdate()
    {
        Vector3 point = CalculatePoint();
        transform.position = Vector3.SmoothDamp(transform.position, point, ref vel, 0.15f);
        transform.LookAt(player, Vector3.up);
    }
    Vector3 CalculatePoint()
    {
        Vector3 direction = -(middlePoint.position - player.position);
        Vector3 point = (middlePoint.position + direction * distance);
        point = new Vector3(Mathf.Clamp(point.x, -125f, 125f), player.position.y + height, Mathf.Clamp(point.z, -125f, 125f));
        return point;
    }
}

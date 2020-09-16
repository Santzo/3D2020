using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBehaviour : MonoBehaviour
{
    [SerializeField]
    public float moveSpeedY, moveSpeedX, maxY, minY, maxX, minX;
    [SerializeField]
    public bool goingUp, goingRight;
    private Vector3 oriPos, maxPos, minPos;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        oriPos = transform.position;
        maxPos = new Vector3(oriPos.x + maxX, oriPos.y + maxY, oriPos.z);
        minPos = new Vector3(oriPos.x - minX, oriPos.y - minY, oriPos.z);
    }
    private void FixedUpdate()
    {
        var move = goingUp ? rb.position + Vector3.up * moveSpeedY * Time.deltaTime :
                             rb.position - Vector3.up * moveSpeedY * Time.deltaTime;
        move = goingRight ? move + transform.right * moveSpeedX * Time.deltaTime :
                                move - transform.right * moveSpeedX * Time.deltaTime;
        rb.MovePosition(move);
        transform.position = rb.position;
        switch (goingUp)
        {
            case false:
                if (rb.position.y < minPos.y)
                    goingUp = !goingUp;
                break;
            case true:
                if (rb.position.y > maxPos.y)
                    goingUp = !goingUp;
                break;
        }
        switch (goingRight)
        {
            case false:
                if (rb.position.x < minPos.x)
                    goingRight = !goingRight;
                break;
            case true:
                if (rb.position.x > maxPos.x)
                    goingRight = !goingRight;
                break;
        }

    }
    void Update()
    {

        // if (!goingUp)
        // {
        //     if (transform.position.y < minPos.y)
        //         goingUp = !goingUp;
        // }

    }
}

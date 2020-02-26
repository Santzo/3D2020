﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 movement;
    bool jump, inAir;
    Transform nose;
    float moveSpeed = 8f;
    float rotationSpeed = 135f;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        nose = transform.Find("Nose");
    }

    private void FixedUpdate()
    {
        if (movement.z != 0)
        { 
            Vector3 move = transform.right * movement.z * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
        if (movement.x != 0)
        {
            Vector3 a = rb.rotation.eulerAngles;
            float rotSpeed = movement.x * rotationSpeed * Time.fixedDeltaTime;
            Quaternion rot = Quaternion.Euler(a.x, a.y + rotSpeed, a.z);
            rb.MoveRotation(rot);
        }
        if (jump && !inAir)
        {
            jump = false;
            Vector3 jumpForce = new Vector3(0f, 225f, 0f);
            rb.AddForce(jumpForce, ForceMode.Impulse);
        }
    }
    private void Update()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (jump) return;
        if (Input.GetKeyDown(KeyCode.Space))
            jump = true;

    }
}


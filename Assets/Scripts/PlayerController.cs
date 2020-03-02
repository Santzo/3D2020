﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 movement;
    bool jump, inAir;
    Transform nose;
    Camera cam;
    float moveSpeed = 280f;
    float rotationSpeed = 135f;
    Rigidbody rb;
    bool controller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        nose = transform.Find("Nose");
    }
    private void Start()
    {
        cam = Camera.main;
        if (Input.GetJoystickNames().Length > 0) controller = true;
        //Debug.Log(Input.GetJoystickNames().Length);
        //foreach (var cont in Input.GetJoystickNames())
        //{
        //    Debug.Log(cont);
        //}
    }

    private void FixedUpdate()
    {
        if (jump && !inAir)
        {
            jump = false;
            inAir = true;
            Vector3 jumpForce = new Vector3(0f, 225f, 0f);
            rb.AddForce(jumpForce, ForceMode.Impulse);
        }

        if (!controller) // Liikkuminen, jos pelaaja pelaa näppäimistöllä
        {
            Vector3 move = transform.right * movement.z * moveSpeed * Time.fixedDeltaTime;
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

            if (movement.x != 0)
            {
                Vector3 a = rb.rotation.eulerAngles;
                float rotSpeed = movement.x * rotationSpeed * Time.fixedDeltaTime;
                Quaternion rot = Quaternion.Euler(a.x, a.y + rotSpeed, a.z);
                rb.MoveRotation(rot);
            }
        }

        else // Liikkuminen, jos pelaaja pelaa ohjaimella
        {
            if (movement.x != 0 || movement.z != 0)
            {
                float rotY = ReturnRotation(movement);
                Quaternion rot = Quaternion.Euler(0f, rotY + cam.transform.eulerAngles.y, 0f);
                Quaternion destRot = Quaternion.Lerp(transform.rotation, rot , 0.25f);
                rb.MoveRotation(destRot);
            }
            float _moveSpeed = Mathf.Min(movement.magnitude, 1f) * moveSpeed * Time.fixedDeltaTime;
            Vector3 move = transform.right * _moveSpeed;
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        }
    }
    private void Update()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (jump || inAir) return;
        if (!controller) // Hyppykoodi, jos pelaaja pelaa näppäimistöllä
        {
            if (Input.GetKeyDown(KeyCode.Space))
                jump = true;
        }
        else
        {
            if (Input.GetButtonDown("BottomButton"))
            {
                jump = true;
            }
        }
    }
    float ReturnRotation(Vector3 rot)
    {
        float angle = Mathf.Atan2(-rot.z, rot.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool onTop = transform.position.y >= collision.transform.position.y;
        inAir = !onTop;
    }
}

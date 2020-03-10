using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector3 movement, collisionPosition;
    bool jump, inAir;
    Transform nose;
    TextMeshProUGUI debug;
    Camera cam;
    float moveSpeed = 280f;
    float rotationSpeed = 135f;
    float jumpForce = 17.5f;
    int jumpFrames = 0, maxJumpFrames = 15;
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
        controller = JoystickHandler.DetectControllerType();
        InputSystem.onDeviceChange += (a, b) => controller = JoystickHandler.DetectControllerType();
        debug = GameObject.Find("Debug").GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        if (jump)
        {
            if (!inAir) Audio.PlaySound("Jump", 0.5f, 0.3f);
            inAir = true;
            jumpFrames++;
            if (jumpFrames <= maxJumpFrames)
            {
                Vector3 _jumpForce = new Vector3(0f, jumpForce, 0f);
                rb.AddForce(_jumpForce, ForceMode.Impulse);
            }
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
            if (movement.x != 0 || movement.y != 0)
            {
                float rotY = ReturnRotation(movement);
                Quaternion rot = Quaternion.Euler(0f, rotY + cam.transform.eulerAngles.y, 0f);
                Quaternion destRot = Quaternion.Lerp(transform.rotation, rot, 0.25f);
                rb.MoveRotation(destRot);
            }
            float _moveSpeed = Mathf.Min(movement.magnitude, 1f) * moveSpeed * Time.fixedDeltaTime;
            Vector3 move = transform.right * _moveSpeed;
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        }
    }
    private void Update()
    {
        debug.text = jumpFrames.ToString();
        movement = controller ? JoystickHandler.Movement : new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) ;


        if (!controller) return;
        JoystickHandler.AnyButton();
        if (JoystickHandler.Attack) Debug.Log("Attack");
        if (JoystickHandler.Jump == 0)
        {
            jumpFrames = 0;
            jump = false;
        }
        if (inAir && jumpFrames == 0 || jumpFrames > maxJumpFrames)
        {
            jump = false;
            return;
        }
        if (!controller) // Hyppykoodi, jos pelaaja pelaa näppäimistöllä
        {
            if (Input.GetKeyDown(KeyCode.Space))
                jump = true;
        }
        else // Hyppykoodi, jos pelaaja pelaa ohjaimella
        {
            if (JoystickHandler.Jump > 0)
            {
                jump = true;
            }
        }

    }
    float ReturnRotation(Vector2 rot)
    {
        float angle = Mathf.Atan2(-rot.y, rot.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool onTop = transform.position.y >= collision.transform.position.y;
        inAir = !onTop;
    }

}


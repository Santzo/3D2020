using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Vector3 movement, collisionPosition;
    bool jump, inAir, onMovingPlatform, isGrounded;
    Vector3 localOffset, shadowOri;
    MovingPlatformBehaviour movingPlatform;
    Transform movingPlatformTransform, shadowTransform;
    Camera cam;
    [SerializeField] float moveSpeed = 280f, rotationSpeed = 0.25f, keyboardRotationSpeed = 0.45f, jumpForce = 17.5f;
    int jumpFrames = 0, maxJumpFrames = 15;
    Rigidbody rb;
    bool controller;
    private int _walkAnimation, _idleAnimation, _attackAnimation, _movementMultiplier, _movementSpeed;
    private int _attackSpeedMultiplier;
    private float movMultiplier;
    private bool attacking;
    private AnimatorClipInfo[] info;
    private int _health;
    private int maxHealth = 8;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            if (value == _health) return;
            if (value > maxHealth) value = maxHealth;
            Events.onPlayerHealthChange(_health, value, maxHealth);
            _health = value;
        }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        HashAnimations();
        shadowTransform = transform.Find("Shadow");
        shadowOri = shadowTransform.localPosition;
    }
    private void Start()
    {
        Health = maxHealth / 2;
        info = animator.GetCurrentAnimatorClipInfo(1);
        cam = Camera.main;
        controller = JoystickHandler.DetectControllerType();
        InputSystem.onDeviceChange += (a, b) => controller = JoystickHandler.DetectControllerType();
    }


    private void FixedUpdate()
    {
        if (jump)
        {
            if (!inAir) Audio.PlaySound("Jump", 0.25f, 0.3f);
            inAir = true;
            jumpFrames++;
            if (jumpFrames <= maxJumpFrames)
            {
                Vector3 _jumpForce = new Vector3(0f, jumpForce, 0f);
                rb.AddForce(_jumpForce, ForceMode.Impulse);
            }
        }
        var magnitude = Mathf.Min(movement.magnitude, 1f);
        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetFloat(_movementSpeed, magnitude);
            animator.SetFloat(_movementMultiplier, movMultiplier * magnitude * 3f);
            float rotY = ReturnRotation(movement) + 90f;
            Quaternion rot = Quaternion.Euler(0f, rotY + cam.transform.eulerAngles.y, 0f);
            Quaternion destRot = Quaternion.Lerp(transform.rotation, rot, controller ? rotationSpeed : keyboardRotationSpeed);
            rb.MoveRotation(destRot);
        }
        float _moveSpeed = magnitude * moveSpeed * Time.fixedDeltaTime;
        Vector3 move = transform.forward * _moveSpeed;
        if (onMovingPlatform)
        {
            move += movingPlatform.goingRight ? movingPlatform.transform.right * movingPlatform.moveSpeedX : -movingPlatform.transform.right * movingPlatform.moveSpeedX;
        }
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
    }

    private void Update()
    {
        movement = controller ? JoystickHandler.Movement : JoystickHandler.KeyboardMovement;
        animator.SetBool(_walkAnimation, movement != Vector3.zero);
        // if (controller) JoystickHandler.AnyButton();
        if (attacking)
        {
            var stateinfo = animator.GetCurrentAnimatorStateInfo(1);
            if (!animator.IsInTransition(1) && stateinfo.IsName("Attack") && stateinfo.normalizedTime >= 1f)
            {
                animator.SetLayerWeight(1, 0f);
                attacking = false;
            }
        }
        if (!attacking && JoystickHandler.Attack)
        {
            animator.SetLayerWeight(1, 1f);
            animator.SetTrigger(_attackAnimation);
            attacking = true;
        }

        if (controller && JoystickHandler.Jump == 0 || !controller && Input.GetKeyUp(KeyCode.Space))
        {
            jumpFrames = 0;
            jump = false;
        }

        if (inAir && jumpFrames == 0 || jumpFrames > maxJumpFrames)
        {
            jump = false;
        }

        switch (jump)
        {
            case true:
                HandleJump();
                break;
            case false:
                if (inAir) break;
                HandleJump();
                break;
        }

        UpdateShadowPosition();
    }
    void HandleJump()
    {
        if (!controller) // Hyppykoodi, jos pelaaja pelaa näppäimistöllä
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jump = true;
            }
        }
        else // Hyppykoodi, jos pelaaja pelaa ohjaimella
        {
            if (JoystickHandler.Jump > 0)
            {
                jump = true;
            }
        }
    }
    void UpdateShadowPosition()
    {
        RaycastHit rayHit;
        var a = Physics.Raycast(transform.position, Vector3.down, out rayHit, 8.5f);
        if (a)
        {
            shadowTransform.position = new Vector3(transform.position.x, rayHit.point.y + 0.15f, transform.position.z);
        }
        else
        {
            shadowTransform.position = Vector3.zero;
        }
    }

    float ReturnRotation(Vector2 rot) => Mathf.Atan2(-rot.y, rot.x) * Mathf.Rad2Deg;

    private void OnCollisionEnter(Collision collision)
    {
        var colPoint = collision.GetContact(0);
        bool onTop = transform.position.y >= colPoint.point.y;
        inAir = !onTop;
        if (collision.collider.CompareTag("MovingPlatform") && onTop)
        {
            movingPlatform = collision.transform.GetComponent<MovingPlatformBehaviour>();
            movingPlatformTransform = movingPlatform.transform;
            onMovingPlatform = true;
        }
        if (collision.collider.CompareTag("DamageTrigger"))
        {
            Health--;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!inAir || jump) return;
        var colPoint = collision.GetContact(0);
        bool onTop = transform.position.y >= colPoint.point.y;
        inAir = !onTop;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("MovingPlatform"))
        {
            onMovingPlatform = false;
        }
    }
    private void HashAnimations()
    {
        _attackAnimation = Animator.StringToHash("Attack");
        _idleAnimation = Animator.StringToHash("Idle");
        _walkAnimation = Animator.StringToHash("Walk");
        _movementMultiplier = Animator.StringToHash("MovementMultiplier");
        _movementSpeed = Animator.StringToHash("MovementSpeed");
        _attackSpeedMultiplier = Animator.StringToHash("AttackSpeed");
        movMultiplier = animator.GetFloat(_movementMultiplier);
        animator.SetFloat(_attackSpeedMultiplier, 8.25f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Meat"))
        {
            Health++;
            other.GetComponent<Meat>().PickUp();
        }
        if (other.CompareTag("Speed"))
        {
            moveSpeed += 55;
            other.GetComponent<Meat>().PickUp();
            movMultiplier += .1f;
            Events.onInfoAction("Speed Increased!");
        }
        if (other.CompareTag("Jump"))
        {
            jumpForce += 2.75f;
            other.GetComponent<Meat>().PickUp();
            Events.onInfoAction("Jump Power Increased!");
        }

    }
}



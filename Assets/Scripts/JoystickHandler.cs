using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class JoystickHandler
{
    public static ButtonControl jump, attack, pause;
    public static Gamepad controller;
    public static bool dualshock;

    public static bool DetectControllerType()
    {
        Debug.Log("KRAAAAAAAAA" + Gamepad.all.Count);
        controller = InputSystem.GetDevice<Gamepad>();
        if (controller.device.name.ToLower().Contains("dualshock"))
        {
            Debug.Log("Playstation controller");
            dualshock = true;
        }
        if (controller != null) SetControls();
        return controller != null;
    }
    public static void SetControls()
    {
        jump = controller.crossButton;
        attack = controller.circleButton;
        pause = controller.startButton;
    }
    public static Vector2 Movement()
    {
        return Gamepad.current.leftStick.ReadValue();
    }
    public static float Jump()
    {
        return jump.ReadValue();
    }
    public static float Attack()
    {
        return attack.ReadValue();
    }
    public static float Start()
    {
        return pause.ReadValue();
    }


}

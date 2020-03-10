using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class JoystickHandler
{
    private static ButtonControl jump, attack, pause;
    private static ButtonControl[] allButtons;
    private static Sprite[] PS4Buttons = Resources.LoadAll<Sprite>("PS4"), XBoxButtons = Resources.LoadAll<Sprite>("XBox");
    private static Image debugImage = GameObject.Find("DebugImage").GetComponent<Image>();
    public static Gamepad controller;
    public static bool dualshock;

    public static bool DetectControllerType()
    {
        controller = InputSystem.GetDevice<Gamepad>();
        if (controller != null)
        {
            if (controller.device.name.ToLower().Contains("dualshock"))
            {
                Debug.Log("Playstation controller");
                dualshock = true;
            }
            else
            {
                Debug.Log("Not a playstation controller");
                dualshock = false;
            }
            SetControls();
            allButtons = (from control in controller.allControls
                         where control is ButtonControl && !control.name.EndsWith("Button")
                         select control as ButtonControl).ToArray();
        }
        return controller != null;
    }
    public static void SetControls()
    {
        jump = controller.buttonSouth;
        attack = controller.buttonWest;
        pause = controller.startButton;
    
    }
    public static Vector2 Movement
    {
        get{
            return controller.leftStick.ReadValue();
        }
    }
    public static float Jump
    {
        get
        {
            return jump.ReadValue();
        }
    }
    public static bool Attack
    {
        get
        {
            return attack.wasPressedThisFrame;
        }
    }
    public static bool Start
    {
        get
        {
            return pause.wasPressedThisFrame;
        }
    }

    public static void AnyButton()
    {
        var pressedButton = allButtons.FirstOrDefault(a => a.wasPressedThisFrame && !a.synthetic);
      
        if (pressedButton != null)
        {
            debugImage.sprite = ButtonSprite(pressedButton.name);
        }
    }

    private static Sprite ButtonSprite(string name)
    {
        return dualshock ? PS4Buttons.FirstOrDefault(a => a.name == name) : XBoxButtons.FirstOrDefault(a => a.name == name);
    }


}

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
    private static ButtonControl jump, attack, pause, cancel;
    private static ButtonControl[] allButtons;
    private static Sprite[] PS4Buttons = Resources.LoadAll<Sprite>("PS4"), XBoxButtons = Resources.LoadAll<Sprite>("XBox");
    private static Image debugImage;
    private static KeyControl[] left, right, up, down;
    private static KeyControl keyJump, keyAttack, keyPause, keyCancel;
    public static Gamepad controller;
    public static bool dualshock;

    public static bool DetectControllerType()
    {
        debugImage = GameObject.Find("DebugImage")?.GetComponent<Image>();
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
        else
        {
            SetKeys();
            Debug.Log("No controller detected.");
        }
        return controller != null;
    }
    private static void SetKeys()
    {
        left = new KeyControl[2];
        right = new KeyControl[2];
        up = new KeyControl[2];
        down = new KeyControl[2];

        left[0] = Keyboard.current.leftArrowKey; left[1] = Keyboard.current.aKey;
        right[0] = Keyboard.current.rightArrowKey; right[1] = Keyboard.current.dKey;
        up[0] = Keyboard.current.upArrowKey; up[1] = Keyboard.current.wKey;
        down[0] = Keyboard.current.downArrowKey; down[1] = Keyboard.current.sKey;
        keyJump = Keyboard.current.spaceKey;
        keyAttack = Keyboard.current.leftShiftKey;
        keyPause = Keyboard.current.escapeKey;
        keyCancel = Keyboard.current.escapeKey;
    }

    private static void SetControls()
    {
        jump = controller.buttonSouth;
        attack = controller.buttonWest;
        pause = controller.startButton;
        cancel = controller.buttonEast;
    }
    
    public static int MenuMovement
    {
        get
        {
            if (controller == null)
            {
                int _up = up[0].wasPressedThisFrame ? -1 : up[1].wasPressedThisFrame ? -1 : 0;
                int _down = down[0].wasPressedThisFrame ? 1 : down[1].wasPressedThisFrame ? 1 : 0;
                return _down + _up; 
            }
            else
            {
                int _up = controller.dpad.up.wasPressedThisFrame ? -1 : controller.leftStick.up.wasPressedThisFrame ? -1 : 0;
                int _down = controller.dpad.down.wasPressedThisFrame ? 1 : controller.leftStick.down.wasPressedThisFrame ? 1 : 0;
                return _down + _up;
            }
        }
    }
    public static Vector2 KeyboardMovement
    {
        get
        {
            float _left = left[0].isPressed ? left[0].ReadValue() : left[1].ReadValue();
            float _right = right[0].isPressed ? right[0].ReadValue() : right[1].ReadValue();
            float _up = up[0].isPressed ? up[0].ReadValue() : up[1].ReadValue();
            float _down = down[0].isPressed ? down[0].ReadValue() : down[1].ReadValue();
                        
            return new Vector2(-_left + _right, -_down + _up);
        }
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
            return controller != null ? jump.ReadValue() : keyJump.ReadValue();
        }
    }
    public static bool Attack
    {
        get
        {
            return controller != null ? attack.wasPressedThisFrame : keyAttack.wasPressedThisFrame;
        }
    }
    public static bool Interact
    {
        get
        {
            return controller != null ? jump.wasPressedThisFrame : keyJump.wasPressedThisFrame;
        }
    }
    public static bool Cancel
    {
        get
        {
            return controller != null ? cancel.wasPressedThisFrame : keyCancel.wasPressedThisFrame;
        }
    }
    public static bool Start
    {
        get
        {
            return controller != null ? pause.wasPressedThisFrame : keyPause.wasPressedThisFrame;
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

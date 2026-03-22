using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputVisualizerInterface : MonoBehaviour
{
    [SerializeField] Canvas gamepadButtons, keyboardMouseButtons;

    void Update()
    {
        bool hasGamepad = Gamepad.current != null;
        bool hasKeyboardMouse = Keyboard.current != null && Mouse.current != null;

        bool displayGamepad = hasGamepad && Gamepad.current.allControls.Any((control) => control.IsActuated());
        bool displayKeyboard = hasKeyboardMouse
            && (Keyboard.current.allControls.Any(control => control.IsActuated())
                || Mouse.current.leftButton.IsActuated() || Mouse.current.rightButton.IsActuated()
            );

        if (displayGamepad)
        {
            gamepadButtons.enabled = true;
            keyboardMouseButtons.enabled = false;
        }
        else if (displayKeyboard)
        {
            keyboardMouseButtons.enabled = true;
            gamepadButtons.enabled = false;
        }
    }
}

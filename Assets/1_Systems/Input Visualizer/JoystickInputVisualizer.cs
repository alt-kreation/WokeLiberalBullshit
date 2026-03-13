using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JoystickInputVisualizer : MonoBehaviour
{
    public InputAction Action, PressAction;

    [SerializeField] RectTransform _joystickImage;
    [SerializeField] Graphic _joystickGraphic;
    [SerializeField] float _range = 10f;
    [SerializeField] Color _activeColor;

    Vector2 _initialPosition;
    Color _initialColor;

    void OnEnable()
    {
        _initialPosition = _joystickImage.anchoredPosition;
        _initialColor = _joystickGraphic.color;

        Action.Enable();
        PressAction.Enable();
    }

    void Update()
    {
        _joystickImage.anchoredPosition = _initialPosition + Action.ReadValue<Vector2>() * _range;
        _joystickGraphic.color = Action.IsPressed() || PressAction.IsPressed()
            ? _activeColor
            : _initialColor;
    }
}

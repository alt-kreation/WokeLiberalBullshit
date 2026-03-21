using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonInputVisualizer : MonoBehaviour
{
    public InputAction Action;

    [SerializeField] RectTransform _buttonTopImage;
    [SerializeField] Graphic _buttonGraphic;
    [SerializeField] float _pressDistance = 20f;
    [SerializeField] Color _activeColor;

    Vector2 _initialPosition;
    Color _initialColor;

    void OnEnable()
    {
        _initialPosition = _buttonTopImage.anchoredPosition;
        _initialColor = _buttonGraphic.color;

        Action.Enable();
    }

    void Update()
    {
        _buttonTopImage.anchoredPosition = Action.IsPressed()
            ? _initialPosition + Vector2.down * _pressDistance
            : _initialPosition;

        _buttonGraphic.color = Action.IsPressed()
            ? _activeColor
            : _initialColor;
    }
}

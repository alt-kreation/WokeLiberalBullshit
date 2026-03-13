using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Allows both value types (bool, int, float, enum, etc) and reference types (string, array, list, etc)
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Variable<T> : ScriptableObject
{
    [SerializeField] private T initialValue;
    [SerializeField] private T currentValue;
    
    public event Action<T> OnValueChanged = delegate { };

    public T Value
    {
        get => currentValue;
        set
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, value)) return; // really cool generics comparer
            this.currentValue = value;
            OnValueChanged.Invoke(value);
        }
    }
    
    #region Resetting
        private void OnEnable()
        {
            Reset();
        #if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        #endif
        }
            
        #if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode) return;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            Reset();
        }
        #endif
        
        protected virtual void Reset()
        {
            currentValue = initialValue;
        }
    #endregion
}
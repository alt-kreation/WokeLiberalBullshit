using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent<T> : ScriptableObject
{
    private readonly List<IGameEventListener<T>> _eventListeners = new();

    public void Raise(T data)
    {
        // reverse in case some stuff in response to this event destroys itself or deregisters which could throw off the list 
        for (int i = _eventListeners.Count - 1; i>= 0; i--)
        {
            _eventListeners[i].OnEventRaised(data);
        }
    }

    public void RegisterListener(IGameEventListener<T> listener)
    {
        _eventListeners.Add(listener);
    }

    public void DeregisterListener(IGameEventListener<T> listener)
    {
        _eventListeners.Remove(listener);
    }
}
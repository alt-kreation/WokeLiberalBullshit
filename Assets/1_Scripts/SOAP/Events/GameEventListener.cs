using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Use this for quick UnityEvent responses
/// </summary>
/// <typeparam name="T"></typeparam>
public class GameEventListener<T> : MonoBehaviour, IGameEventListener<T>
{
	[SerializeField] private GameEvent<T> _gameEvent;
	[SerializeField] private UnityEvent<T> _eventResponse;

	private void OnEnable()
	{
		_gameEvent.RegisterListener(this);
	}

	private void OnDisable()
	{
		_gameEvent.DeregisterListener(this);
	}

	public void OnEventRaised(T data)
	{
		_eventResponse.Invoke(data);
	}
}
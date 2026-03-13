using System;
using UnityEngine;

public class PlayerTriggerSensor : MonoBehaviour
{
	public static event Action<GameObject> OnPlayerTriggerEnter;
	public static event Action<GameObject> OnPlayerTriggerExit;
	
	private void OnTriggerEnter(Collider other)
	{
		OnPlayerTriggerEnter?.Invoke(other.gameObject);
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		OnPlayerTriggerEnter?.Invoke(other.gameObject);
	}

	private void OnTriggerExit(Collider other)
	{
		OnPlayerTriggerExit?.Invoke(other.gameObject);
	}
	
	private void OnTriggerExit2D(Collider2D other)
	{
		OnPlayerTriggerExit?.Invoke(other.gameObject);
	}
}
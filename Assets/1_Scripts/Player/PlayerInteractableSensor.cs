using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Void.Input;
using Void.NPC;

public class PlayerInteractableSensor : MonoBehaviour
{
	public static event Action<Interactable> OnObjectInteractedWith; 
	public static Interactable InteractableObjectInRange { get; private set; }
	[SerializeField] private bool _useNotificationSystem;
	
	[SerializeField] private InteractableTypeGameObjectDictionary _notificationDictionary;
	private GameObject _interactableNotification => GetNotificationToShow();
	[SerializeField] protected virtual InputReader _inputReader { get; set; }

	private void OnEnable()
	{
		ShouldShowNotification(false);

		PlayerTriggerSensor.OnPlayerTriggerEnter += HandlePlayerTriggerEnter;
		PlayerTriggerSensor.OnPlayerTriggerExit += HandlePlayerTriggerExit;
		
		RegisterInput();
	}

	private void OnDisable()
	{
		PlayerTriggerSensor.OnPlayerTriggerEnter -= HandlePlayerTriggerEnter;
		PlayerTriggerSensor.OnPlayerTriggerExit -= HandlePlayerTriggerExit;
		
		DeregisterInput();
	}
	
	protected virtual void RegisterInput()
	{
		_inputReader.OnInteractEvent += () => HandleInteract(new InputAction.CallbackContext());
	}
	
	protected virtual void DeregisterInput()
	{
		_inputReader.OnInteractEvent -= () => HandleInteract(new InputAction.CallbackContext());
	}

	protected virtual void HandleInteract(InputAction.CallbackContext input)
	{
		if (InteractableObjectInRange == null) return;
		if (!InteractableObjectInRange.IsInteractable) return;

		InteractableObjectInRange.Interact();
		OnObjectInteractedWith?.Invoke(InteractableObjectInRange);
	}

	//amending this as a real jank priority system
	//otherwise you drop an item when you start a conversation
	//then the item becomes the object in range and you can't progress dialogue
	private void HandlePlayerTriggerEnter(GameObject colliderObject)
	{
		if (!colliderObject.TryGetComponent(out Interactable interactable)) return;

		// If we already have an NPC in range and a new item enters, ignore the item
		if (InteractableObjectInRange is BaseNPC && interactable is PickupableItem)
		{
			return;
		}

		//this is to stop getting stuck in the map view screen if you dropped an interactable item while next to it
		if (InteractableObjectInRange is MapViewer && interactable is PickupableItem)
		{
			return;
		}

		// If an NPC enters while we have an item in range, prioritize the NPC
		if (interactable is BaseNPC)
		{
			InteractableObjectInRange = interactable;
			if (_useNotificationSystem)
			{
				StartCoroutine(Co_ObjectInRangeChecks());
			}
			return;
		}

		InteractableObjectInRange = interactable;

		if (!_useNotificationSystem) return;
		StartCoroutine(Co_ObjectInRangeChecks());
	}

	private IEnumerator Co_ObjectInRangeChecks()
	{
		while (InteractableObjectInRange != null)
		{
			ShouldShowNotification(InteractableObjectInRange.IsInteractable);
			yield return new WaitForSeconds(0.1f);
		}
		
		ShouldShowNotification(false);
	}

	private void HandlePlayerTriggerExit(GameObject colliderObject)
	{
		if (!colliderObject.TryGetComponent(out Interactable interactable)) return;

		if (interactable != InteractableObjectInRange) return;
		
		InteractableObjectInRange = null;
	}

	private void ShouldShowNotification(bool shouldShow)
	{
		if (_interactableNotification == null)
		{
			DisableAllNotifications();
			return;
		}
		
		if (_interactableNotification.activeInHierarchy == shouldShow) return;
		
		DisableAllNotifications();
		_interactableNotification.SetActive(shouldShow);
	}

	private void DisableAllNotifications()
	{
		// disable all 
		var values = new List<GameObject>(_notificationDictionary.Values);
		for (int i = 0; i < values.Count; i++)
		{
			values[i].SetActive(false);
		}
	}

	private GameObject GetNotificationToShow()
	{
		if (InteractableObjectInRange == null) return null;

		if (_notificationDictionary.TryGetValue(InteractableObjectInRange.MyInteractableType, out GameObject notificationObject))
		{
			return notificationObject;
		}

		Debug.LogWarning($"Failed to find notification object for {InteractableObjectInRange.MyInteractableType}");
		return null;
	}
}

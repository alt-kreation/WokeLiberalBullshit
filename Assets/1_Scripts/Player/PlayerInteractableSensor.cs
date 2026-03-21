using System;
using FishingIsland;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractableSensor : MonoBehaviour
{
	public static event Action<Interactable> OnObjectInteractedWith; 
	public static Interactable InteractableObjectInRange { get; private set; }
	
	[SerializeField] private GridMovement2D _gridMovement2D;

	private void OnValidate()
	{
		_gridMovement2D = GetComponent<GridMovement2D>();
	}
	
	private void OnEnable()
	{
		PlayerTriggerSensor.OnPlayerTriggerEnter += HandlePlayerTriggerEnter;
		PlayerTriggerSensor.OnPlayerTriggerExit += HandlePlayerTriggerExit;

		InputHandler.FishingActions.Overworld.Interact.performed += HandleInteract;
	}

	private void OnDisable()
	{
		PlayerTriggerSensor.OnPlayerTriggerEnter -= HandlePlayerTriggerEnter;
		PlayerTriggerSensor.OnPlayerTriggerExit -= HandlePlayerTriggerExit;
		
		InputHandler.FishingActions.Overworld.Interact.performed -= HandleInteract;
	}

	protected virtual void HandleInteract(InputAction.CallbackContext input)
	{
		// can't interact while moving
		if (_gridMovement2D.IsMoving) return;
		
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
		
		InteractableObjectInRange = interactable;
	}


	private void HandlePlayerTriggerExit(GameObject colliderObject)
	{
		if (!colliderObject.TryGetComponent(out Interactable interactable)) return;

		if (interactable != InteractableObjectInRange) return;
		
		InteractableObjectInRange = null;
	}
}

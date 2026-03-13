using System;
using UnityEngine;

// potential to turn into database of strings 
public enum InteractableType
{
	Default,
	Pickupable,
	Conversant,
	Examinable,
	Usable,
	Boat
}

public abstract class Interactable : MonoBehaviour
{
	public virtual bool IsInteractable { get; protected set; } = true;
	public virtual InteractableType MyInteractableType { get; protected set; } = InteractableType.Default;
	
	public virtual void Interact()
	{
		if (!IsInteractable) return;
	}
	
	/// <summary>
	/// Enable and Disable the object's interactivity 
	/// </summary>
	/// <param name="shouldBeInteractable"></param>
	public void SetInteractable(bool shouldBeInteractable)
	{
		IsInteractable = shouldBeInteractable;
	}
}
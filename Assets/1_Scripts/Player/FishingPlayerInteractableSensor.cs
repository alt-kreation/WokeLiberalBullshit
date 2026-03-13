using UnityEngine;
using UnityEngine.InputSystem;
using Void.Input;

namespace FishingIsland
{
	public class FishingPlayerInteractableSensor : PlayerInteractableSensor
	{
		// doesn't use the InputReader system
		protected override InputReader _inputReader => null;

		[SerializeField] private GridMovement2D _gridMovement2D;

		private void OnValidate()
		{
			_gridMovement2D = GetComponent<GridMovement2D>();
		}

		protected override void RegisterInput()
		{
			InputHandler.FishingActions.Overworld.Interact.performed += HandleInteract;
		}

		protected override void DeregisterInput()
		{
			InputHandler.FishingActions.Overworld.Interact.performed -= HandleInteract;
		}
		
		protected override void HandleInteract(InputAction.CallbackContext input)
		{
			if (_gridMovement2D.IsMoving) return;
			
			base.HandleInteract(input);
		}
	}
}

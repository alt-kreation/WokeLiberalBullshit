using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingIsland
{
	public static class InputHandler
	{
		private static bool DEBUG_MODE = false;

		public static event Action OnActiveActionMapsChanged;
		public static FishingIsland_Controls FishingActions { get; private set; }
		public static HashSet<InputActionMap> ActiveActionMaps { get; private set; } = new HashSet<InputActionMap>();
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			FishingActions = new FishingIsland_Controls();
			FishingActions.Disable();
		}
		
		public static void EnableActionMap(InputActionMap actionMap)
		{
			if (actionMap.enabled) return;
			
			actionMap.Enable();
			ActiveActionMaps.Add(actionMap);

			if (DEBUG_MODE)
			{
				Debug.Log($"Activating {actionMap.name}. Maps Active: {ActiveActionMaps.Count}");
			}
			
			OnActiveActionMapsChanged?.Invoke();
		}

		public static void DisableAllActionMaps()
		{
			FishingActions.Disable();
			ActiveActionMaps.Clear();

			if (DEBUG_MODE)
			{
				Debug.Log($"All maps disabled!");
			}
			
			OnActiveActionMapsChanged?.Invoke();
		}
	}
}
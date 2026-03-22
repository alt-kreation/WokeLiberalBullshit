using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputHandler
{
	private static bool DEBUG_MODE = true;

	public static event Action OnActiveActionMapsChanged;
	public static Controls ActionAsset { get; private set; }
	public static HashSet<InputActionMap> ActiveActionMaps { get; private set; } = new HashSet<InputActionMap>();
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		ActionAsset = new Controls();
		ActionAsset.Disable();
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
		ActionAsset.Disable();
		ActiveActionMaps.Clear();

		if (DEBUG_MODE)
		{
			Debug.Log($"All maps disabled!");
		}
		
		OnActiveActionMapsChanged?.Invoke();
	}
}
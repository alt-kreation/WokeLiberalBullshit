using FishingIsland;
using UnityEditor;
using UnityEngine;

public static class DatabaseGetter
{
#if UNITY_EDITOR
	public static CharacterNamesDatabase CharacterNamesData => GetDatabase<CharacterNamesDatabase>();
	public static MoveAnimationNameDatabase MoveAnimationNameData => GetDatabase<MoveAnimationNameDatabase>();
	public static YarnNodeNameDatabase YarnNodeNamesData => GetDatabase<YarnNodeNameDatabase>();

	private static T GetDatabase<T>() where T : ScriptableObject
	{
		string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
		
		if (guids.Length > 1)
		{
			Debug.LogWarning($"Found more than one {typeof(T).Name}. There should only be one.");
		}
		else if (guids.Length == 0)
		{
			Debug.LogError($"No asset found of type {typeof(T).Name}.");
			return null;
		}

		// Load asset by its path
		string path = AssetDatabase.GUIDToAssetPath(guids[0]);
		T database = AssetDatabase.LoadAssetAtPath<T>(path);

		return database;
	}
#endif
}
using System.Linq;
using EditorAttributes;
#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.SceneManagement;
#endif
using UnityEngine;
	
#if UNITY_EDITOR
public class DEBUG_PositionEvaluator : MonoBehaviour
{
	[SerializeField] private int _shouldBeDivisibleBy;
	[SerializeField] private Transform[] _excludedTransforms;
	
	[Button("ValidatePositions")]
	public void EvaluatePositions()
	{
		Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		int invalidPositionsCount = 0;

		foreach (Transform foundTransform in allTransforms)
		{
			if (_excludedTransforms.Contains(foundTransform)) continue;
			
			if (!Utils2D.CheckIfNumDivisibleBy(foundTransform.position.x, _shouldBeDivisibleBy) || !Utils2D.CheckIfNumDivisibleBy(foundTransform.position.y, _shouldBeDivisibleBy))
			{
				Debug.LogWarning($"Object '{foundTransform.name}' has invalid position: {foundTransform.position}");
				invalidPositionsCount++;
			}
		}

		if (invalidPositionsCount == 0)
		{
			Debug.Log($"Checked {allTransforms.Length - _excludedTransforms.Length} transforms. All object positions are valid");
		}
		else
		{
			Debug.LogWarning($"Found {invalidPositionsCount} objects with invalid positions");
		}
	}

	// note this kinda messes up trying to move children of parents because moving a parent offsets a child
	[Button("SnapPositions")]
	public void SnapPositions()
	{
		Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		int fixedCount = 0;

		Undo.RecordObjects(allTransforms, "Fix Object Positions"); // Makes the operation undoable

		foreach (Transform foundTransform in allTransforms)
		{
			if (_excludedTransforms.Contains(foundTransform)) continue;
			
			if (!Utils2D.CheckIfNumDivisibleBy(foundTransform.position.x, _shouldBeDivisibleBy))
			{
				Vector3 currentPos = foundTransform.position;
				float snappedX = Mathf.Round(currentPos.x / _shouldBeDivisibleBy/100) * _shouldBeDivisibleBy/100;
				foundTransform.position = new Vector3(snappedX, currentPos.y, currentPos.z);
				
				// Handle prefab instances
				if (PrefabUtility.IsPartOfPrefabInstance(transform))
				{
					// Record the position change as a prefab override
					PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
				}
				
				fixedCount++;
			}
		}

		if (fixedCount > 0)
		{
			// Mark scene as dirty to ensure changes are saved
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			Debug.Log($"Fixed {fixedCount} object positions");
		}
		else
		{
			Debug.Log("No positions needed fixing");
		}
	}
}
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using EditorAttributes;
using UnityEngine;
using Yarn.Unity;

//[CreateAssetMenu(fileName = "YarnNodeNameDatabase", menuName = "Scriptable Objects/YarnNodeNameDatabase")]
public class YarnNodeNameDatabase : ScriptableObject
{
	[SerializeField] internal YarnProject _yarnProject;
	[field: Tooltip("Just visible for Debugging purposes.")]
	[field: SerializeField] public string[] NodeNames { get; private set; }
	
	#if UNITY_EDITOR
	private void OnValidate()
	{
		//RefreshDatabase();
	}
	#endif


	[Button]
	private void RefreshDatabase()
	{
		NodeNames = new string[_yarnProject.NodeNames.Length];
		for (int i = 0; i < _yarnProject.NodeNames.Length; i++)
		{
			NodeNames[i] = _yarnProject.NodeNames[i].Replace('_', '/');
		}
		
		#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
		#endif
	}
}
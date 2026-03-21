using UnityEngine;

//[CreateAssetMenu(fileName = "CharacterNameDatabase", menuName = "Scriptable Objects/CharacterNameDatabase")]
public class CharacterNamesDatabase : ScriptableObject
{
    [field: SerializeField] public string[] CharacterNames { get; private set; }
}
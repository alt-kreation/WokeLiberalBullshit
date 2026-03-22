using EditorAttributes;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New PresentationPersonality", menuName = "Data/New PresentationPersonality")]
public class PresentationPersonality: ScriptableObject
{
    [field: Header("Personality Parameters")]
    [field: Tooltip("Leave this blank to use default font")]
    [field: SerializeField] public TMP_FontAsset FontOverride { get; protected set; }
    [field: SerializeField] public bool UseTypeWriter { get; protected set; }
    [field: Tooltip("Leave this blank for no voice")]
    [field: SerializeField, ShowField(nameof(UseTypeWriter))] public VoiceData Voice { get; private set; }
    [field: SerializeField, HideField(nameof(UseTypeWriter))] public AudioClip InteractSFX { get; private set; }
    [field: SerializeField, HideField(nameof(UseTypeWriter))] public bool UseInteractSFXOnAdvance { get; private set; }
}

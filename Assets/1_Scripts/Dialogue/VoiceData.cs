using EditorAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New_VoiceData", menuName = "Data/New VoiceData")]
public class VoiceData : ScriptableObject
{
    [field: SerializeField, Range(0, 1)] public float Volume { get; private set; } = 0.75f;
    [field: SerializeField, Range(-3, 3)] public float Pitch { get; private set; } = 1f;
    [field: SerializeField, Range(-3, 3)] public float SpeedMultiplier { get; private set; } = 1f;
    [field: SerializeField] public AudioClip[] VocalSounds { get; private set; }
    [field: Tooltip("True if sounds will be picked random from the array\nFalse if they'll be chosen in order"), SerializeField]
    public bool PlayInRandomOrder { get; private set; }
    [field: Tooltip("True if you want the new sound to cut the previous one\nFalse if each sound will continue until its end"), SerializeField]
    public bool InterruptPreviousSound { get; private set; }
    [field: Tooltip("How much time has to pass before playing the next sound"), SerializeField,  Clamp(0f, 2f)]
    public float MinSoundDelay { get; private set; } = 0.7f;
}
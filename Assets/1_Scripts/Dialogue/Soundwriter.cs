using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Febucci.UI.Examples
{
    /// <summary>
    /// Based on the example class from Febucci to add sounds TextAnimators using typewriters
    /// </summary>
    [AddComponentMenu("Febucci/TextAnimator/SoundWriter")]
    [RequireComponent(typeof(Core.TypewriterCore))]
    public class TAnimSoundWriter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioSource Source;
        [field: SerializeField] public VoiceData DefaultVoiceData { get; private set; }

        private VoiceData _activeVoice = null;
        
        float _latestTimePlayed = -1;
        int _clipIndex;

        #region DEBUG
        [Header("Debug")] 
        [SerializeField] private float _soundTestDuration;
        [SerializeField] private VoiceData _voiceToTest;
        private Coroutine _debugTestSoundRoutine;
        
        [ContextMenu(nameof(DEBUG_TestSoundwriter))]
        private void DEBUG_TestSoundwriter()
        {
            if (_debugTestSoundRoutine != null) return;
            _debugTestSoundRoutine = StartCoroutine(Co_TestSound());
        }
        
        private IEnumerator Co_TestSound()
        {
            if (!Application.IsPlaying(this))
            {
                Debug.Log("Must test in playmode! Editor Coroutines are extra faff");
                _debugTestSoundRoutine = null;
                yield break;
            }
            
            ApplyVoiceData(_voiceToTest);

            float elapsedTime = 0f;

            while (elapsedTime < _soundTestDuration)
            {
                elapsedTime += Time.deltaTime;
                OnCharacter('i');

                yield return null;
            }
            
            ApplyVoiceData(DefaultVoiceData);
            _debugTestSoundRoutine = null;
        }
        #endregion

        private void Awake()
        {
            Validation();
            Initialize();
        }

        private void Validation()
        {
            Assert.IsNotNull(Source, "TAnimSoundWriter: Typewriter Audio Source reference is null");
            Assert.IsNotNull(DefaultVoiceData, "No default voice set!!");
            Assert.IsNotNull(GetComponent<Core.TypewriterCore>(), "TAnimSoundWriter: Component TAnimPlayerBase is not present");
        }

        private void Initialize()
        {
            if (_activeVoice == null)
            {
                ApplyVoiceData(DefaultVoiceData);
            }

            //Prevents common setup errors
            Source.playOnAwake = false;
            Source.loop = false;

            GetComponent<Core.TypewriterCore>()?.onCharacterVisible.AddListener(OnCharacter);
        }

        public void ApplyVoiceData(VoiceData voice)
        {
            _activeVoice = voice;
            if (_activeVoice == null) return;
            
            Source.volume = voice.Volume;
            Source.pitch = voice.Pitch;
            
            _clipIndex = _activeVoice.PlayInRandomOrder ? Random.Range(0, _activeVoice.VocalSounds.Length) : 0;
        }

        void OnCharacter(char character)
        {
            if (!_activeVoice) return;
            
            if (character == '.' || char.IsWhiteSpace(character))
                return;

            if (Time.time - _latestTimePlayed <= _activeVoice.MinSoundDelay)
                return; //Early return if not enough time passed yet

            Source.clip = _activeVoice.VocalSounds[_clipIndex];

            //Plays audio
            if (_activeVoice.InterruptPreviousSound)
                Source.Play();
            else
                Source.PlayOneShot(Source.clip);

            //Chooses next clip to play
            if (_activeVoice.PlayInRandomOrder)
            {
                _clipIndex = Random.Range(0, _activeVoice.VocalSounds.Length);
            }
            else
            {
                _clipIndex++;
                if (_clipIndex >= _activeVoice.VocalSounds.Length)
                    _clipIndex = 0;
            }

            _latestTimePlayed = Time.time;
        }
    }
}
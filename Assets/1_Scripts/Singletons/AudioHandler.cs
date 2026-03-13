using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace FishingIsland
{
	public class AudioHandler : Singleton<AudioHandler>
	{
		[SerializeField] private AudioSource _levelTrackSource;
		[SerializeField] private AudioSource _schedulableTrackSource;
		[SerializeField] private AudioSource _sfxSource;
		[SerializeField] private AudioSource _schedulableSFXSource;

		private float _mainMusicVolume;
		private float _dippedMusicVolume => _mainMusicVolume * 0.25f;

		private Coroutine _fadeMusicRoutine;
		

		public void PlayMusic(AudioClip musicToPlay, float volume)
		{
			if (_levelTrackSource.clip == musicToPlay && _levelTrackSource.isPlaying) return;
			
			_mainMusicVolume = volume;
			_levelTrackSource.volume = volume;
			_levelTrackSource.clip = musicToPlay;
			_levelTrackSource.Play();
		}

		public void StopMusic()
		{
			_levelTrackSource.Stop();
			_schedulableTrackSource.Stop();
		}

		public void PlaySFX(AudioClip sfxToPlay, float volume)
		{
			_sfxSource.PlayOneShot(sfxToPlay, volume);
		}
		
		public void PlayMusicScheduled(AudioClip musicClip, float volume, double delay, bool stopOtherMusic)
		{
			if (_schedulableTrackSource.clip == musicClip && _schedulableTrackSource.isPlaying) return;
			
			_mainMusicVolume = volume;
			_schedulableTrackSource.volume = volume;
			_schedulableTrackSource.clip = musicClip;
			
			// Schedule playback using DSP time for precision
			double scheduleTime = AudioSettings.dspTime + delay;
			_schedulableTrackSource.PlayScheduled(scheduleTime);

			if (stopOtherMusic)
			{
				StartCoroutine(Co_TurnOffDelay());
			}
			
			IEnumerator Co_TurnOffDelay()
			{
				while (AudioSettings.dspTime < scheduleTime)
				{
					yield return null;
				}
				_levelTrackSource.Stop();
			}
		}
		
	}
}

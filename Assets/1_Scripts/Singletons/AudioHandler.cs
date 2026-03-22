using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class AudioHandler : Singleton<AudioHandler>
{
	[Header("Music Sources")]
	[SerializeField] private AudioSource _musicSource;
	[SerializeField] private AudioSource _schedulableMusicSource;
	
	[Header("SFX Sources")]
	[SerializeField] private AudioSource _sfxSource;
	[SerializeField] private AudioSource _schedulableSFXSource;
	
	[field: Header("Dialogue Sources")]
	[field: Tooltip("This will be requested by DialogueHandler to use for Typewriter Voices")]
	[field: SerializeField] public AudioSource TypewriterVoiceSource { get; private set; }

	TweenerCore<float, float, FloatOptions> _fadeTween = null;
		
	#region Music
	
	public void PlayMusic(AudioClip musicToPlay, float volume)
	{
		if (_musicSource.clip == musicToPlay && _musicSource.isPlaying) return;
		
		_musicSource.volume = volume;
		_musicSource.clip = musicToPlay;
		_musicSource.Play();
	}

	public void StopMusic()
	{
		_musicSource.Stop();
		_schedulableMusicSource.Stop();
	}
	
	public void PlayMusicScheduled(AudioClip musicClip, float volume, double delay, bool stopOtherMusic)
	{
		if (_schedulableMusicSource.clip == musicClip && _schedulableMusicSource.isPlaying) return;

		_schedulableMusicSource.volume = volume;
		_schedulableMusicSource.clip = musicClip;
		
		// Schedule playback using DSP time for precision
		double scheduleTime = AudioSettings.dspTime + delay;
		_schedulableMusicSource.PlayScheduled(scheduleTime);

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
			_musicSource.Stop();
		}
	}

	public void FadeMusic(float targetVolume, float transitionTime)
	{
		if (_fadeTween != null)
		{
			Debug.LogWarning("Requested to fade music, but a fade tween was already playing. Killing old one.");
			_fadeTween.Kill();
		}
		
		if (_musicSource.isPlaying)
		{
			_fadeTween = _musicSource.DOFade(targetVolume, transitionTime).SetEase(Ease.OutSine);
		}

		if (_schedulableMusicSource.isPlaying)
		{
			_fadeTween ??= _schedulableMusicSource.DOFade(targetVolume, transitionTime).SetEase(Ease.OutSine);
		}
		
		_fadeTween?.OnComplete(() => _fadeTween = null);
	}
	
	#endregion
	
	#region SFX
	
	public void PlaySFX(AudioClip sfxToPlay, float volume)
	{
		_sfxSource.PlayOneShot(sfxToPlay, volume);
	}

	public void PlaySFXScheduled(AudioClip sfxClip, float volume, double delay)
	{
		if (_schedulableSFXSource.clip == sfxClip && _schedulableSFXSource.isPlaying) return;
		if (_schedulableSFXSource.isPlaying && _schedulableSFXSource.clip != sfxClip)
		{
			Debug.LogWarning($"Scheduled sfx source is already playing a different clip! Requesting {sfxClip}, Playing {_schedulableSFXSource.clip}");
			return;
		}

		_schedulableSFXSource.volume = volume;
		_schedulableSFXSource.clip = sfxClip;
		
		// Schedule playback using DSP time for precision
		double scheduleTime = AudioSettings.dspTime + delay;
		_schedulableSFXSource.PlayScheduled(scheduleTime);
	}

	#endregion
}
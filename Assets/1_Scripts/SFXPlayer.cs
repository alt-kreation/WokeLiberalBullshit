using UnityEngine;

namespace FishingIsland
{
	/// <summary>
	/// Used for playing SFX through Unity events
	/// </summary>
	public class SFXPlayer : MonoBehaviour
	{
		[SerializeField] private float _volume = 0.7f;
		
		public void PlaySFX(AudioClip clip)
		{
			AudioHandler.Instance.PlaySFX(clip, _volume);
		}

		/// <summary>
		/// seperated into 2 methods because UnityEvents in inspector only support 0 or 1 params
		/// </summary>
		/// <param name="volume"></param>
		public void SetVolume(float volume)
		{
			_volume = volume;
		}
	}
}

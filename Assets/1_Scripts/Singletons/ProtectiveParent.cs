using UnityEngine;

namespace FishingIsland
{
	public class ProtectiveParent : Singleton<ProtectiveParent>
	{
		protected override void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}
	}
}
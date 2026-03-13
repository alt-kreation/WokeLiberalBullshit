using System.Collections;
using UnityEngine;

namespace FishingIsland
{
	[SelectionBase]
	public class PlayerWarper : MonoBehaviour
	{
		[SerializeField] private Vector2 _posToWarpPlayerTo;
		[SerializeField] private bool _isDestinationInterior;
		[SerializeField] private float _timeToWarp;
		[Space]
		[SerializeField] private CameraMaskChange _maskChanger;

		private Coroutine _warpRoutine;

		private void OnValidate()
		{
			_maskChanger = FindFirstObjectByType<CameraMaskChange>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_warpRoutine != null) return;
			if (!other.TryGetComponent(out PlayerTransform pt)) return;

			_warpRoutine = StartCoroutine(Co_WarpPlayer());
		}

		private IEnumerator Co_WarpPlayer()
		{
			InputHandler.DisableAllActionMaps();
			InputHandler.EnableActionMap(InputHandler.FishingActions.Empty);

			if (PlayerTransform.Transform.gameObject.TryGetComponent(out GridMovement2D movement))
			{
				yield return new WaitUntil(() => !movement.IsMoving);
			}
			
			PlayerTransform.Transform.position = _posToWarpPlayerTo;

			if (_isDestinationInterior)
			{
				_maskChanger.ApplyInteriorMask(0);
			}

			else
			{
				_maskChanger.ApplyDefaultMask(0);
			}
			
			InputHandler.DisableAllActionMaps();
			InputHandler.EnableActionMap(InputHandler.FishingActions.Overworld);

			_warpRoutine = null;
		}
	}
}

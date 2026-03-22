using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class GridMovement2D : MonoBehaviour
{
	public bool IsMoving => _moveTween != null;
	
	[SerializeField] private float _tileSize = 0.16f;
	[Tooltip("How long it takes to move to the next tile")]
	[SerializeField] private float _timeToMove = 0.2f;
	[SerializeField] private Ease _moveEase;
	[Space]
	[Tooltip("Offset for how to check for if a tile is available")]
	[SerializeField] private Vector2 _offset;

	private Transform _transform;
	
	private Vector2 _movementInput;
	private float _lastHorizontalInputTime = -1f;
	private float _lastVerticalInputTime = -1f;
	
	private Vector2 _originalPos, _targetPos;

	private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;

	private void OnValidate()
	{
		GetComponent<Rigidbody2D>().gravityScale = 0;
	}

	private void OnEnable()
	{
		_transform = transform;
	}

	private void FixedUpdate()
	{
		Movement();
	}
	
	private void Movement()
	{
		// return if already in a movement routine
		if (_moveTween != null) return;

		GetLatestInput();
		
		// no need to check if valid tile if no input
		if (_movementInput == Vector2.zero) return;
		
		_originalPos = _transform.position;
		_targetPos = _originalPos + (_movementInput * _tileSize);

		if (!CheckIfCanMoveToTile()) return;

		if (_moveTween == null)
		{
			_moveTween = _transform.DOMove(_targetPos, _timeToMove)
				.SetEase(_moveEase)
				.SetAutoKill(true)
				.OnComplete(ValidatePos);
		}
	}

	private void GetLatestInput()
	{
		float horizontalInput = InputHandler.ActionAsset.Overworld.Move.ReadValue<Vector2>().x;
		float verticalInput = InputHandler.ActionAsset.Overworld.Move.ReadValue<Vector2>().y;

		// keep track of when last input pressed
		if (horizontalInput != 0 && _movementInput.x == 0)
		{
			_lastHorizontalInputTime = Time.time;
		}
		if (verticalInput != 0 && _movementInput.y == 0)
		{
			_lastVerticalInputTime = Time.time;
		}

		// Reset movement input
		_movementInput = Vector2.zero;

		// if both directions pressed, check which one was latest
		if (horizontalInput != 0 && verticalInput != 0)
		{
			if (_lastHorizontalInputTime > _lastVerticalInputTime)
			{
				_movementInput.x = horizontalInput;
			}
			else
			{
				_movementInput.y = verticalInput;
			}
		}
		else
		{
			// for when only one direction is pressed
			if (horizontalInput != 0)
			{
				_movementInput.x = horizontalInput;
			}
			if (verticalInput != 0)
			{
				_movementInput.y = verticalInput;
			}
		}
	}
	
	private bool CheckIfCanMoveToTile()
	{
		Vector2 size = new Vector2(_tileSize - 0.1f, _tileSize - 0.1f);

		ContactFilter2D filter = new ContactFilter2D();
		filter.useTriggers = false;
		filter.SetLayerMask(~LayerMask.GetMask("Player"));

		Collider2D[] results = new Collider2D[1];
		int numColliders = Physics2D.OverlapBox(_targetPos + _offset, size, 0f, filter, results);

		if (numColliders > 0)
		{
			//Debug.Log($"Cant go into tile with {results[0].gameObject.name}");
		}

		return numColliders == 0;
	}
	
	private void ValidatePos()
	{
		_moveTween = null; 
		
		if (Utils2D.CheckIfNumDivisibleBy(_transform.position.x, 16) && Utils2D.CheckIfNumDivisibleBy(_transform.position.y, 16)) return;

		Vector2 correctedPos = _transform.position;
		if (!Utils2D.CheckIfNumDivisibleBy(_transform.position.x, 16))
		{
			correctedPos.x = Utils2D.GetNearestNumberDivisibleBy(_transform.position.x, 0.16f);
		}

		if (!Utils2D.CheckIfNumDivisibleBy(_transform.position.y, 16))
		{
			correctedPos.y = Utils2D.GetNearestNumberDivisibleBy(_transform.position.y, 0.16f);
		}

		_transform.position = correctedPos;
	}
	
	private void OnDrawGizmos()
	{
		if (_movementInput == Vector2.zero) return;
		
		Vector2 size = new Vector2(_tileSize, _tileSize);

		// Draw the box where we're checking for collisions
		Gizmos.color = Color.cyan;
		Vector2 target = _targetPos + _offset;
		Gizmos.DrawWireCube((Vector3)target, (Vector3)size);
	}
}
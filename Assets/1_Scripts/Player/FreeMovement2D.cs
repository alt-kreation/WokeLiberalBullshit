using UnityEngine;

namespace FishingIsland
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class FreeMovement2D : MonoBehaviour
	{
		[SerializeField] private float _moveSpeed = 5f;
		[SerializeField] private Transform _gfx;
	
		private Rigidbody2D _rb;      
		private Vector2 _movement;

		private void OnValidate()
		{
			GetComponent<Rigidbody2D>().gravityScale = 0;
		}

		private void Start()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			Movement();
		}

		private void Movement()
		{
			_movement.x = Input.GetAxisRaw("Horizontal");
			_movement.y = Input.GetAxisRaw("Vertical");
		
			RotateGFX();
			ApplyMovement();
		}

		private void RotateGFX()
		{
			if (_movement.x == 0) return;

			float yRotation = _movement.x > 0 ? 0 : 180;
			_gfx.transform.rotation = Quaternion.Euler(0, yRotation, 0);
		}

		private void ApplyMovement()
		{
			_movement = _movement.normalized;
			_rb.linearVelocity = _movement * (_moveSpeed * Time.deltaTime);
		}
	}
}

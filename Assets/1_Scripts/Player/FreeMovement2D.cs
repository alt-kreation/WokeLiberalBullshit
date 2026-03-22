using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FreeMovement2D : MonoBehaviour
{
	// NOTE TO SELF : use a CircleCollider2D instead of a BoxCollider2D for this
	// also set Pixel Perfect Camera Grid Snapping to none if the camera is childed
	
	[SerializeField] private float _moveSpeed = 1f;
	[SerializeField] private Transform _gfx;

	private Rigidbody2D _rb;      
	private Vector2 _movement;

	private void Start()
	{
		Initialization();
	}
	
	private void Initialization()
	{
		_rb = GetComponent<Rigidbody2D>();
		_rb.gravityScale = 0;
		_rb.bodyType = RigidbodyType2D.Dynamic;
	}

	private void FixedUpdate()
	{
		Movement();
	}

	private void Movement()
	{
		_movement = InputHandler.ActionAsset.Overworld.Move.ReadValue<Vector2>();
	
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
		_rb.linearVelocity = _movement * _moveSpeed;
	}
}

using UnityEngine;

public class PlayerTransform : MonoBehaviour
{
	public static Transform Transform;

	private void Awake()
	{
		Transform = transform;
	}
}
using EditorAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSavePosition : MonoBehaviour
{
	private Transform _transform;
	private static Vector2 _savedPosition = Vector2.zero;

	private void OnEnable()
	{
		_transform = transform;

		if (_savedPosition != Vector2.zero)
		{
			_transform.position = _savedPosition;
		}
	}

	private void OnDisable()
	{
		_savedPosition = _transform.position;
	}

	[Button]
	public void DEBUG_ReloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}

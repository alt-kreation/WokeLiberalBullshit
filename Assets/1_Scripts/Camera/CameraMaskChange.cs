using System.Collections;
using EditorAttributes;
using UnityEngine;

public class CameraMaskChange : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _interiorMask;
    [SerializeField] private LayerMask _defaultMask;

    private Coroutine _maskChangeRoutine;
    
    [Button]
    public void ApplyInteriorMask(float delay)
    {
        if (_maskChangeRoutine != null) return;

        _maskChangeRoutine = StartCoroutine(Co_InteriorDelay(delay));
    }

    private IEnumerator Co_InteriorDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
                
        _camera.clearFlags = CameraClearFlags.SolidColor;
        _camera.backgroundColor = Color.black;
        
        _camera.cullingMask = _interiorMask.value;
        
        _maskChangeRoutine = null;
    }

    [Button]
    public void ApplyDefaultMask(float delay)
    {
        if (_maskChangeRoutine != null) return;

        _maskChangeRoutine = StartCoroutine(Co_DefaultDelay(delay));
    }
    
    private IEnumerator Co_DefaultDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        _camera.clearFlags = CameraClearFlags.Skybox;
        _camera.cullingMask = _defaultMask.value;

        _maskChangeRoutine = null;
    }
}
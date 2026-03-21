using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TypewriterEventListener : MonoBehaviour
{
    [SerializeField] private string _eventToListenFor;
    [SerializeField] private UnityEvent OnEventHeard;
    
    private void Awake()
    {
        StartCoroutine(Co_GetDialogueHandler());
    }

    private IEnumerator Co_GetDialogueHandler()
    {
        yield return new WaitUntil(() => DialogueHandler.Instance != null);
        DialogueHandler.Instance.Typewriter.onMessage.AddListener(OnTypewriterMessage);
    }

    private void OnDisable()
    {
        if (DialogueHandler.Instance != null)
        {
            DialogueHandler.Instance.Typewriter.onMessage.RemoveListener(OnTypewriterMessage);
        }
    }
    
    private void OnTypewriterMessage(Febucci.UI.Core.Parsing.EventMarker eventMarker)
    {
        if (!eventMarker.name.Equals(_eventToListenFor)) return;
        
        OnEventHeard?.Invoke();
    }
}

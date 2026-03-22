using UnityEngine;

/// <summary>
/// add this to an object to leave a comment on it
/// </summary>
public class Comment : MonoBehaviour
{
    [SerializeField, TextArea] private string _comment;
}

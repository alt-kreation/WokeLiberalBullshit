using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "SOAP/Events/NoParam_GameEvent")]
// how to raise a parameterless event with a null object 
public class NoParam_GameEvent : GameEvent<Unit>
{
    public void Raise() => Raise(Unit.Default);
}

// a NullObject to raise parameterless events 
public struct Unit
{
    public static Unit Default => default;
}
using UnityEngine;

public class OverworldInputSetter : MonoBehaviour
{
    private void OnEnable()
    {
        InputHandler.DisableAllActionMaps();
        InputHandler.EnableActionMap(InputHandler.ActionAsset.Overworld);
    }
}

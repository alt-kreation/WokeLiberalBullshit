using UnityEngine;
using UnityEngine.AddressableAssets;

public class InitializeAddressableOnPlay
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSingleton()
    {        
        if (ProtectiveParent.Instance == null)
        {
            Addressables.InstantiateAsync("Singletons").WaitForCompletion();
        }
    }
}
using System;
using UnityEngine;

namespace FishingIsland
{
    public class DialogueProgressIcon : MonoBehaviour
    {
        private void OnEnable()
        {
            // if conversation action map isn't enabled, this object shouldn't be shown because it's a icon telling player to hit progress
            if (!InputHandler.ActiveActionMaps.Contains(InputHandler.FishingActions.Conversation))
            {
                gameObject.SetActive(false);
            }
        }
    }
}

using System.Collections;
using EditorAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingIsland
{
    [SelectionBase]
	public class FishingNPC : Interactable
	{
        [Dropdown(nameof(_nodeNames))]
        [SerializeField] protected string StartNode;
        
        [field: Header("Presentation Personality")]
        [field: SerializeField] public PresentationPersonality Personality { get; private set; }

        #region NodeNamesDatabaseRef
        [HideInInspector, SerializeField] protected YarnNodeNameDatabase _nodeNameDatabase;
        protected string[] _nodeNames => _nodeNameDatabase.NodeNames;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_nodeNameDatabase == null)
            {
                _nodeNameDatabase = DatabaseGetter.YarnNodeNamesData;
            }
        }
#endif
        #endregion

        private void OnDisable()
        {
            EventSubscription(false);
        }

        public void SetStartNode(string newNode)
        {
            StartNode = newNode;
        }

        public void SetPresentationPersonality(PresentationPersonality personality)
        {
            Personality = personality;
            FishDialogueHandler.Instance.ApplyPersonalityToPresenter(personality);
        }
        
        private void HandleProgress(InputAction.CallbackContext obj)
        {
            FishingInteract();
        }

        public override void Interact()
        {
            FishingInteract();
        }

        /// <summary>
        /// useConversatoinActionMap to say if you want the player to use ConversationMap or if progressing is outside player's control
        /// </summary>
        /// <param name="useConversationActionMap"></param>
        public void FishingInteract(bool useConversationActionMap = true)
        {
            // if it's autoadvancing, reject input bc in automode
            if (FishDialogueHandler.Instance.IsAutoAdvancing) return;
            if (!FishDialogueHandler.Instance.IsDialogueRunning && !FishDialogueHandler.Instance.IsTransitioning)
            {
                base.Interact();
                EventSubscription(true);
                
                FishDialogueHandler.Instance.StartDialogue(StartNode, useConversationActionMap);
                FishDialogueHandler.Instance.ApplyPersonalityToPresenter(Personality);
                
                if (!Personality.UseTypeWriter && Personality.InteractSFX != null)
                {
                    AudioHandler.Instance.PlaySFX(Personality.InteractSFX, 0.7f);
                }
            }
            else if (FishDialogueHandler.Instance.IsDialogueRunning && !FishDialogueHandler.Instance.IsTransitioning)
            {
                // if a typewriter effect is going, show the rest of the text with a button press instead of progressing
                if (FishDialogueHandler.Instance.IsShowingText)
                {
                    FishDialogueHandler.Instance.ActiveLinePresenter.Typewriter.SetTypewriterSpeed(150f);
                    return;
                }
                
                FishDialogueHandler.Instance.RequestNextLine();

                if (_interactDelay == null)
                {
                    _interactDelay = StartCoroutine(Co_InteractDelay());
                }

                else
                {
                    StopCoroutine(_interactDelay);
                    _interactDelay = StartCoroutine(Co_InteractDelay());
                }
            }
        }

        private Coroutine _interactDelay;
        /// <summary>
        /// allows the Personality Parameters to catch up and apply themselves if they're changed Conversation using "set_conversant_style"
        /// </summary>
        /// <returns></returns>
        private IEnumerator Co_InteractDelay()
        {
            yield return null;
            
            if (Personality.UseTypeWriter && Personality.Voice != null)
            {
                FishDialogueHandler.Instance.ActiveLinePresenter.Typewriter.SetTypewriterSpeed(Personality.Voice.SpeedMultiplier);
            }
                
            if (!Personality.UseTypeWriter && Personality.InteractSFX != null && Personality.UseInteractSFXOnAdvance)
            {
                AudioHandler.Instance.PlaySFX(Personality.InteractSFX, 0.7f);
            }

            _interactDelay = null;
        }

        /// <summary>
        /// Runs interact, but lets you wait until the conversation is complete
        /// </summary>
        /// <returns></returns>
        public IEnumerator Co_AwaitableInteract(bool useConversationActionMap = true)
        {
            FishingInteract(useConversationActionMap);
            yield return new WaitUntil(() => FishDialogueHandler.Instance.IsDialogueRunning);
            yield return new WaitUntil(() => !FishDialogueHandler.Instance.IsDialogueRunning);
        }

        //I've added these to give NPC reactivity to the Service events but it may end up that
        //this is handled differently for instance through Yarn Commands for common dialogue reactions
        private void OnFishingDialogueStarted() { }

        private void OnFishingDialogueProgressed() { }

        private void OnFishingDialogueComplete()
        {
            //any npc specific stuff to do when dialogue ends
            EventSubscription(false);
        }

        private void EventSubscription(bool sub)
        {
            if (FishDialogueHandler.Instance == null) return;
            
            if (sub)
            {
                InputHandler.FishingActions.Conversation.Progress.performed += HandleProgress;

                FishDialogueHandler.Instance.OnDialogueStart += OnFishingDialogueStarted;
                FishDialogueHandler.Instance.OnDialogueProgressed += OnFishingDialogueProgressed;
                FishDialogueHandler.Instance.OnDialogueComplete += OnFishingDialogueComplete;
            }
            else
            {
                FishDialogueHandler.Instance.OnDialogueStart -= OnFishingDialogueStarted;
                FishDialogueHandler.Instance.OnDialogueProgressed -= OnFishingDialogueProgressed;
                FishDialogueHandler.Instance.OnDialogueComplete -= OnFishingDialogueComplete;
                
                InputHandler.FishingActions.Conversation.Progress.performed -= HandleProgress;
            }
        }
    }
}
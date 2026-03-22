using System.Collections;
using EditorAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
public class NPC : Interactable
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
        DialogueHandler.Instance.ApplyPersonalityToPresenter(personality);
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
        if (DialogueHandler.Instance.IsAutoAdvancing) return;
        if (!DialogueHandler.Instance.IsDialogueRunning && !DialogueHandler.Instance.IsTransitioning)
        {
            base.Interact();
            EventSubscription(true);
            
            DialogueHandler.Instance.StartDialogue(StartNode, useConversationActionMap);
            DialogueHandler.Instance.ApplyPersonalityToPresenter(Personality);
            
            if (!Personality.UseTypeWriter && Personality.InteractSFX != null)
            {
                AudioHandler.Instance.PlaySFX(Personality.InteractSFX, 0.7f);
            }
        }
        else if (DialogueHandler.Instance.IsDialogueRunning && !DialogueHandler.Instance.IsTransitioning)
        {
            // if a typewriter effect is going, show the rest of the text with a button press instead of progressing
            if (DialogueHandler.Instance.IsShowingText)
            {
                DialogueHandler.Instance.ActiveLinePresenter.FebucciTypewriter.SetTypewriterSpeed(150f);
                return;
            }
            
            DialogueHandler.Instance.RequestNextLine();

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
            DialogueHandler.Instance.ActiveLinePresenter.FebucciTypewriter.SetTypewriterSpeed(Personality.Voice.SpeedMultiplier);
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
        yield return new WaitUntil(() => DialogueHandler.Instance.IsDialogueRunning);
        yield return new WaitUntil(() => !DialogueHandler.Instance.IsDialogueRunning);
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
        if (DialogueHandler.Instance == null) return;
        
        if (sub)
        {
            InputHandler.ActionAsset.Conversation.Progress.performed += HandleProgress;

            DialogueHandler.Instance.OnDialogueStart += OnFishingDialogueStarted;
            DialogueHandler.Instance.OnDialogueProgressed += OnFishingDialogueProgressed;
            DialogueHandler.Instance.OnDialogueComplete += OnFishingDialogueComplete;
        }
        else
        {
            DialogueHandler.Instance.OnDialogueStart -= OnFishingDialogueStarted;
            DialogueHandler.Instance.OnDialogueProgressed -= OnFishingDialogueProgressed;
            DialogueHandler.Instance.OnDialogueComplete -= OnFishingDialogueComplete;
            
            InputHandler.ActionAsset.Conversation.Progress.performed -= HandleProgress;
        }
    }
}
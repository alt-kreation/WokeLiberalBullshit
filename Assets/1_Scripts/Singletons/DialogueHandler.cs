using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class DialogueHandler : Singleton<DialogueHandler>
{
    public event Action OnDialogueStart;
    public event Action OnDialogueProgressed;
    public event Action OnDialogueComplete;
    public VoidLinePresenter ActiveLinePresenter => GetActivePresenter();
    public bool IsShowingText => GetActivePresenter().FebucciTypewriter.IsShowingText;
    public bool IsDialogueRunning => _dialogueRunner.IsDialogueRunning;
    public bool IsTransitioning => CheckIfTransitioning();
    
    public bool IsAutoAdvancing { get; private set; }
    
    [Header("Yarn Components")]
    [SerializeField] private DialogueRunner _dialogueRunner;
    
    private Coroutine _startingFocusedDialogueRoutine;

    private HashSet<InputActionMap> _cachedPreviousMaps = null;

    private VoidLinePresenter GetActivePresenter()
    {
        foreach (var dialoguePresenter in _dialogueRunner.DialoguePresenters)
        {
            if (dialoguePresenter is VoidLinePresenter presenter && presenter.gameObject.activeInHierarchy)
            {
                return presenter;
            }
        }
        
        Debug.LogWarning("No active line presenter!");

        return null;
    }
    
    private void OnEnable()
    {
        _dialogueRunner.onDialogueComplete?.AddListener(ExitDialogue);
        SetActiveLinePresenter(LinePresenterType.Overworld);
    }

    private void OnDisable()
    {
        _dialogueRunner.onDialogueComplete?.RemoveAllListeners();
    }

    public void SetActiveLinePresenter(LinePresenterType type)
    {
        // reset this just in case
        SetCustomAutoAdvance(false);

        foreach (var dialoguePresenter in _dialogueRunner.DialoguePresenters)
        {
            if (dialoguePresenter is VoidLinePresenter presenter && presenter.Type == type)
            {
                presenter.gameObject.SetActive(true);
            }
            else
            {
                dialoguePresenter?.gameObject.SetActive(false);
            }
        }
    }
    
    public void StartDialogue(string nodeName, bool useConversationActionMap = true)
    {
        if (!_dialogueRunner)
        {
            Debug.LogWarning("No Active Dialogue Runner Set!");
            return;
        }
        
        _dialogueRunner.StartDialogue(nodeName);
        OnDialogueStart?.Invoke();

        if (!useConversationActionMap) return;
        _cachedPreviousMaps = new HashSet<InputActionMap>(InputHandler.ActiveActionMaps);
        InputHandler.DisableAllActionMaps();
        InputHandler.EnableActionMap(InputHandler.ActionAsset.Conversation);
    }

    public void ApplyPersonalityToPresenter(PresentationPersonality personality)
    {
        ActiveLinePresenter.ApplyPersonalityParameters(personality);
    }
    
    public void RequestNextLine()
    {
        // clear text so can't see it when transitioning
        ActiveLinePresenter.FebucciTypewriter.TextAnimator.SetText(String.Empty);
        
        OnDialogueProgressed?.Invoke();
        _dialogueRunner.RequestNextLine();
    }

    public void ExitDialogue()
    {
        if (!IsDialogueRunning) return;
        
        if (_cachedPreviousMaps != null)
        {
            InputHandler.DisableAllActionMaps();
            
            foreach (var cachedMap in _cachedPreviousMaps)
            {
                InputHandler.EnableActionMap(cachedMap);
            }
            
            _cachedPreviousMaps = null;
        }
        
        _dialogueRunner.Stop();
        OnDialogueComplete?.Invoke();

        if (IsAutoAdvancing)
        {
            ActiveLinePresenter.FebucciTypewriter.onTextShowed.RemoveListener(RequestNextLine);
            IsAutoAdvancing = false;
        }
        
        ActiveLinePresenter.ResetPersonalityParameters();
    }
    
    // Can expand this out with more stuff as we add more effects to how dialogue is shown
    private bool CheckIfTransitioning()
    {
        if (_startingFocusedDialogueRoutine != null)
        {
            return true;
        }
        
        bool isLinePresenterAnimating = false;
        foreach (var dialoguePresenter in _dialogueRunner.DialoguePresenters)
        {
            if (dialoguePresenter is VoidLinePresenter presenter && presenter.IsAnimating())
            {
                isLinePresenterAnimating = true;
            }
        }

        if (isLinePresenterAnimating)
        {
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// done outside of Yarn's auto advance because of autoAdvanceDelay.
    /// there was no way to tell how long that delay had to be because it's reliant on how long typewriter takes to show
    /// </summary>
    /// <param name="value"></param>
    [YarnCommand("set_auto_advance")]
    public void SetCustomAutoAdvance(bool value)
    {
        IsAutoAdvancing = value;
        
        if (IsAutoAdvancing)
        {
            ActiveLinePresenter.FebucciTypewriter.onTextShowed.AddListener(RequestNextLine);
        }

        else
        {
            ActiveLinePresenter.FebucciTypewriter.onTextShowed.RemoveListener(RequestNextLine);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionManager0thPuzzle : InteractionManager<IInteractableBehaviour0thPuzzle> //to be changed to 0thPuzzle
{
    public static InteractionManager0thPuzzle Instance { get; private set; }

    public event EventHandler OnWorldViewActivated;
    public event EventHandler OnPlatformViewActivated;

    [SerializeField] private Transform puzzle0ObjectsHolder;

    private enum GameState
    {
        WorldView,
        BookPlatformView
    }

    public List<Interactable0thPuzzleObject> allBooks;
    public List<Interactable0thPuzzleObject> representativeBooks;

    private GameState currentState = GameState.WorldView;
    private Interactable0thPuzzlePlatform bookPlatform;
    private InteractionPanelIndividual InteractionPanelIndividual;
    private bool wasUIActivatedPreviousFrame;
    private bool isUIActivatedThisFrame;

    private void Awake()
    {
        SetInstance();
    }

    protected override void Start()
    {
        base.Start();
        InitializeObjects();
        OnObjectCollidersApproached += PlayerInteractionManager_ObjectCollidersApproached;
        OnNoInteractableNear += PlayerInteractionManager_NoInteractableNear;
        OnInteractableApproached += PlayerInteractionManager_InteractableApproached;
        OnWorldViewActivated += InteractionManager0thPuzzle_ActiveViewChanged;
        OnPlatformViewActivated += InteractionManager0thPuzzle_ActiveViewChanged;
        CameraManager.Instance.SetCameraViewToPuzzle0();
    }

    private void Update()
    {
        isUIActivatedThisFrame = false;
        DetectAnyColliderApproached();
        DetectInteractionConditionsMet();

        if (!isUIActivatedThisFrame && wasUIActivatedPreviousFrame) //Deactivate only if previously activated
        {
            Debug.Log("Changed from activated to deactivated");
            TriggerInteractionPanelIndividualDeactivated(this);
        }
        wasUIActivatedPreviousFrame = isUIActivatedThisFrame;
    }

    public Transform GetObjectsHolderTransform()
    {
        return puzzle0ObjectsHolder;
    }

    public Transform GetPlatformTransform()
    {
        return bookPlatform.transform;
    }

    public Vector3 GetPosToPutBookOnPlatform(Interactable0thPuzzleObject book)
    {
        return bookPlatform.GetNextBookPos();
    }

    protected override void DetectInteractionConditionsMet()
    {
        if (currentState == GameState.WorldView)
        {
            DetectInteractionConditionsMet_WorldView();
        }
        else if (currentState == GameState.BookPlatformView)
        {
            DetectInteractionConditionsMet_BookPlatformView();
        }
    }

    private void DetectInteractionConditionsMet_WorldView()
    {
        if (AreHandsFull())
        {
            DetectIfBookPlaceNear_HandsFull();
            DetectIfBookPlatformNear_HandsFull();
        }
        else
        {
            DetectIfBooksNear_HandsEmpty();
            DetectIfBookPlatformNear_HandsEmpty();
        }
    }

    private void DetectInteractionConditionsMet_BookPlatformView()
    {
        
        if (bookPlatform.IsExitPlatformViewKeyPressed())
        {
            OnWorldViewActivated?.Invoke(this, null);
        }
        else
        {
            DetectPickUpPointedBook();
        }
    }

    private void DetectIfBookPlaceNear_HandsFull()
    {
        if (interactableInHand is Interactable0thPuzzleObject bookInHand)
        {
            if (IsNear(bookInHand.GetBookPlaceOnShelfTransform()))
            {
                InteractableBehaviourPutOnShelf putOnShelfBehaviour = new();
                RaiseInteractionConditionsMet(this, bookInHand, putOnShelfBehaviour);
            }
        }
    }

    private void DetectIfBookPlatformNear_HandsFull()
    {
        //Debug.Log("Put book on platform check");
        if (interactableInHand is Interactable0thPuzzleObject bookInHand)
        {
            if (IsNear(bookPlatform.transform))
            {
                if (bookPlatform.HasPlace()) //A book can be placed on platform with other books
                {
                    InteractableBehaviourPutOnBookPlatform putOnBookPlatformBehaviour = new();
                    RaiseInteractionConditionsMet(this, interactableInHand, putOnBookPlatformBehaviour);
                }
            }
        }
    }

    private void DetectIfBooksNear_HandsEmpty() //Handled by event OnInteractableApproached
    {
        List<Interactable<IInteractableBehaviour0thPuzzle>> nearInteractables = GetNearInteractablesList(nearColliders);
        if (nearInteractables.Count > 0)
        {
            InteractableBehaviourPickUpFromShelf pickUpFromShelf = new();
            Interactable0thPuzzleObject nearestBook = GetNearestBookToScreen(nearInteractables);
            if (nearestBook != null)
                RaiseInteractionConditionsMet(this, nearestBook, pickUpFromShelf);
        }
    }

    private void DetectIfBookPlatformNear_HandsEmpty()
    {
        //Debug.Log("Take book from platform check");
        if (IsNear(bookPlatform.transform))
        {
            if (bookPlatform.HasBooks()) //Any book can be got into hand when at least one book exists on platform
            {
                DetectPlatformView();
            }
        }
    }

    private void DetectPlatformView()
    {
        //Display UI for the key to be pressed when game switches to platform view
        DisplayEnterPlatformViewUI();

        if (bookPlatform.IsEnterPlatformViewKeyPressed())
        {
            OnPlatformViewActivated?.Invoke(this, null);
        }
    }

    private void DisplayEnterPlatformViewUI()
    {
        Transform platformTopTransform = bookPlatform.GetTopCenterPointTransform();
        KeyCode platformViewEnterKey = bookPlatform.GetEnterPlatformViewKey();
        TriggerInteractionPanelIndividualActivated(this, platformTopTransform, platformViewEnterKey);
    }


    protected override void PlayerInteractionManager_InteractableApproached(object sender, Interactable<IInteractableBehaviour0thPuzzle> interactable)
    {
        if (interactable is Interactable0thPuzzleObject)
        {
            Interactable0thPuzzleObject book = interactable as Interactable0thPuzzleObject;
            foreach (IInteractableBehaviour0thPuzzle interactionBehaviour in interactable.GetInteractionBehaviours())
            {
                if (!bookPlatform.HasTheBook(book))
                {
                    InteractableBehaviourPickUpFromShelf pickUpFromShelf = new();
                    RaiseInteractionConditionsMet(sender, book, pickUpFromShelf);
                }
            }
        }
    }

    protected override void InteractionManager_InteractionConditionsMet(object sender, InteractionBehaviourEventArgs e)
    {
        DetectBehaviourApplied(e.InteractedObject, e.InteractionBehaviour);
    }

    protected override void InteractionManager_InteractableInteracted(object sender, InteractionBehaviourEventArgs e)
    {
        if (e.InteractionBehaviour is InteractableBehaviourPickUpFromBookPlatform)
        {
            OnWorldViewActivated?.Invoke(this, null);
        }
        
        base.InteractionManager_InteractableInteracted(sender, e);
    }

    private void InteractionManager0thPuzzle_ActiveViewChanged(object sender, EventArgs e)
    {
        ToggleViewAndUpdate();
    }

    private void DetectPickUpPointedBook()
    {
        Interactable0thPuzzleObject pointedBook = bookPlatform.GetPointedBook();
        if (pointedBook != null)
        {
            InteractableBehaviourPickUpFromBookPlatform pickUpFromPlatform = new();
            RaiseInteractionConditionsMet(this, pointedBook, pickUpFromPlatform);
        }
    }

    //A new list is created with the books that are not on platform (the distinction is made here) //TO DO LATER
    private Interactable0thPuzzleObject GetNearestBookToScreen(List<Interactable<IInteractableBehaviour0thPuzzle>> nearInteractables)
    {
        foreach (Interactable0thPuzzleObject book in nearInteractables.OfType<Interactable<IInteractableBehaviour0thPuzzle>>())
        {
            if (book != null && !bookPlatform.HasTheBook(book))
            {
                return book;
            }
        }

        return null;
    }

    private void TriggerInteractionPanelIndividualActivated(object sender, InteractionBehaviourEventArgs e)
    {
        isUIActivatedThisFrame = true;

        Transform targetTransform = e.InteractedObject.transform;
        KeyCode behaviourKey = e.InteractionBehaviour.InteractionKeyCode;
        InteractionPanelIndividual.RaiseInteractionPanelActivated(sender, targetTransform, behaviourKey);
    }

    private void TriggerInteractionPanelIndividualActivated(object sender, Transform targetTransform, KeyCode targetKey)
    {
        isUIActivatedThisFrame = true;

        InteractionPanelIndividual.RaiseInteractionPanelActivated(sender, targetTransform, targetKey);
    }

    private void TriggerInteractionPanelIndividualDeactivated(object sender)
    {
        InteractionPanelIndividual.RaiseInteractionPanelDeactivated(sender);
    }

    private void ToggleViewAndUpdate()
    {
        CameraManager.Instance.SwitchToNextCamera();
        ChangeGameState();
        MouseManager.Instance.ChangeMouseVisibility();
        if (PlayerMovementManager.Instance.enabled) PlayerScriptsManager.Instance.DisableMovementScript();
        else PlayerScriptsManager.Instance.EnableMovementScript();
    }

    private void ChangeGameState()
    {
        currentState = currentState == GameState.WorldView ? GameState.BookPlatformView : GameState.WorldView;
    }
    
    private void SetGameState(GameState gameState)
    {
        currentState = gameState;
    }

    private void InitializeObjects()
    {
        bookPlatform = Interactable0thPuzzlePlatform.Instance;
        InteractionPanelIndividual = InteractionPanelIndividual.Instance;
        isUIActivatedThisFrame = false;
        wasUIActivatedPreviousFrame = false;
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
}

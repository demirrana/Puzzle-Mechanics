using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionManager0thPuzzle : InteractionManager<IInteractableBehaviour0thPuzzle> //to be changed to 0thPuzzle
{
    #region Singleton and Events
    public static InteractionManager0thPuzzle Instance { get; private set; }

    public event EventHandler OnWorldViewActivated;
    public event EventHandler OnPlatformViewActivated;

    private enum GameState
    {
        WorldView,
        BookPlatformView
    }
    #endregion

    #region Fields
    [SerializeField] private Transform puzzle0ObjectsHolder; //objects are children of this transform in the world view

    public List<Interactable0thPuzzleObject> allBooks;
    public List<Interactable0thPuzzleObject> representativeBooks;

    private List<Collider> nearColliders = new();

    private GameState currentState = GameState.WorldView;
    private Interactable0thPuzzlePlatform bookPlatform;
    private InteractionPanelMovable InteractionPanelIndividual;
    private bool wasUIActivatedPreviousFrame;
    private bool isUIActivatedThisFrame;
    #endregion

    #region Lifecycle Methods
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

        if (!isUIActivatedThisFrame && wasUIActivatedPreviousFrame) //Deactivate interaction panel only if previously activated
        {
            //Debug.Log("Changed from activated to deactivated");
            TriggerInteractionPanelIndividualDeactivated(this);
        }
        wasUIActivatedPreviousFrame = isUIActivatedThisFrame;
    }

    private void OnDestroy()
    {
        OnObjectCollidersApproached -= PlayerInteractionManager_ObjectCollidersApproached;
        OnNoInteractableNear -= PlayerInteractionManager_NoInteractableNear;
        OnInteractableApproached -= PlayerInteractionManager_InteractableApproached;
        OnWorldViewActivated -= InteractionManager0thPuzzle_ActiveViewChanged;
        OnPlatformViewActivated -= InteractionManager0thPuzzle_ActiveViewChanged;
    }
    #endregion

    #region Getter Methods
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
    #endregion

    #region Book Interaction Detection Methods
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
        if (AreHandsFull()) //either put book on shelf or put book on platform
        {
            DetectIfBookShelfNear_HandsFull();
            DetectIfBookPlatformNear_HandsFull();
        }
        else //either pick book up from shelf or pick book up from platform
        {
            DetectIfBooksNear_HandsEmpty();
            DetectIfBookPlatformNear_HandsEmpty();
        }
    }

    //Either pick book up from platform or switch to world view
    private void DetectInteractionConditionsMet_BookPlatformView()
    {
        //Debug.Log("Platform view");
        if (bookPlatform.IsExitPlatformViewKeyPressed())
        {
            OnWorldViewActivated?.Invoke(this, null);
        }
        else
        {
            DetectPickUpPointedBook();
        }
    }

    private void DetectIfBookShelfNear_HandsFull()
    {
        //Debug.Log("Put book on shelf check");
        if (interactableInHand is Interactable0thPuzzleObject bookInHand)
        {
            if (IsNear(bookInHand.GetBookPlaceOnShelfTransform()))
            {
                //Debug.Log("book place near");
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

    private void DetectIfBooksNear_HandsEmpty()
    {
        //Debug.Log("DetectIfBooksNear_HandsEmpty method call");
        List<Interactable<IInteractableBehaviour0thPuzzle>> nearInteractables = GetNearInteractablesList(nearColliders);
        if (nearInteractables.Count > 0)
        {
            InteractableBehaviourPickUpFromShelf pickUpFromShelf = new();
            Interactable0thPuzzleObject nearestBook = GetNearestBookToScreen(nearInteractables);
            if (nearestBook != null)
                RaiseInteractionConditionsMet(this, nearestBook, pickUpFromShelf);
        }
        //Debug.Log("Take book from shelf check");
    }

    private void DetectIfBookPlatformNear_HandsEmpty()
    {
        //Debug.Log("Take book from platform check");
        if (IsNear(bookPlatform.transform))
        {
            //Debug.Log("platform near");
            if (bookPlatform.HasBooks()) //Any book can be got into hand when at least one book exists on platform
            {
                //Debug.Log("platform has books");
                DetectPlatformView();
            }
        }
    }

    private void DetectPlatformView()
    {
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

    private void DetectPickUpPointedBook()
    {
        Interactable0thPuzzleObject pointedBook = bookPlatform.GetPointedBook();
        if (pointedBook != null)
        {
            InteractableBehaviourPickUpFromBookPlatform pickUpFromPlatform = new();
            RaiseInteractionConditionsMet(this, pointedBook, pickUpFromPlatform);
        }
    }
    #endregion

    #region Event Handlers
    //Keep track of the near colliders 
    protected override void PlayerInteractionManager_ObjectCollidersApproached(object sender, List<Collider> colliderList)
    {
        nearColliders = colliderList;
        base.PlayerInteractionManager_ObjectCollidersApproached(sender, colliderList);
    }

    protected override void PlayerInteractionManager_NoInteractableNear(object sender, EventArgs e)
    {
        if (nearColliders.Count != 0)
        {
            nearColliders.Clear();
        }
    }

    protected override void InteractionManager_InteractionConditionsMet(object sender, InteractionBehaviourEventArgs e)
    {
        TriggerInteractionPanelBasedOnBehaviour(sender, e);
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
        TogglePlayerMesh();
    }
    #endregion

    #region Event Raising Methods

    private void TriggerInteractionPanelIndividualActivated(object sender, InteractionBehaviourEventArgs e)
    {
        //Debug.Log("panel activated");
        isUIActivatedThisFrame = true;

        Transform targetTransform = e.InteractedObject.transform;
        KeyCode behaviourKey = e.InteractionBehaviour.InteractionKeyCode;
        InteractionPanelIndividual.RaiseInteractionPanelActivated(sender, targetTransform, behaviourKey);
    }

    private void TriggerInteractionPanelIndividualActivated(object sender, Transform targetTransform, KeyCode targetKey)
    {
        //Debug.Log("panel activated for non interactable object");
        isUIActivatedThisFrame = true;

        InteractionPanelIndividual.RaiseInteractionPanelActivated(sender, targetTransform, targetKey);
    }

    private void TriggerInteractionPanelBasedOnBehaviour(object sender, InteractionBehaviourEventArgs e)
    {
        switch(e.InteractionBehaviour)
        {
            case InteractableBehaviourPickUpFromShelf pickUpFromShelf:
                TriggerInteractionPanelIndividualActivated(sender, e);
                break;
            case InteractableBehaviourPickUpFromBookPlatform pickUpFromPlatform:
                TriggerInteractionPanelIndividualActivated(sender, e);
                break;
            case InteractableBehaviourPutOnBookPlatform putOnBookPlatform:
                TriggerInteractionPanelIndividualActivated(sender, bookPlatform.GetTopCenterPointTransform(), bookPlatform.GetEnterPlatformViewKey());
                break;
            case InteractableBehaviourPutOnShelf putOnShelf:
                Interactable0thPuzzleObject book = e.InteractedObject as Interactable0thPuzzleObject;
                if (book != null)
                {
                    Transform initialPlace = book.GetBookPlaceOnShelfTransform();
                    TriggerInteractionPanelIndividualActivated(sender, initialPlace, e.InteractionBehaviour.InteractionKeyCode);
                }
                break;
            default:
                break;
        }
    }

    private void TriggerInteractionPanelIndividualDeactivated(object sender)
    {
        //Debug.Log("panel deactivated");
        InteractionPanelIndividual.RaiseInteractionPanelDeactivated(sender);
    }
    #endregion

    #region Helper Methods
    private void ToggleViewAndUpdate()
    {
        //Debug.Log("ToggleViewAndUpdate method call");
        CameraManager.Instance.SwitchToNextCamera();
        ChangeGameState();
        MouseManager.Instance.ChangeMouseVisibility();
        if (PlayerMovementManager.Instance.enabled) PlayerScriptsManager.Instance.DisableMovementScript();
        else PlayerScriptsManager.Instance.EnableMovementScript();
    }

    private void TogglePlayerMesh() //stop displaying player when platform is shown (camera is between player and platform)
    {
        MeshRenderer playerMesh = PlayerMovementManager.Instance.GetComponentInChildren<MeshRenderer>();
        playerMesh.enabled = !playerMesh.enabled;
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
        InteractionPanelIndividual = InteractionPanelMovable.Instance;
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
    #endregion
}
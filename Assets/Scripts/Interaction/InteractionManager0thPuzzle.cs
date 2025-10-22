using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager0thPuzzle : InteractionManager<IInteractableBehaviour0thPuzzle> //to be changed to 0thPuzzle
{
    public static InteractionManager0thPuzzle Instance { get; private set; }

    [SerializeField] private Transform puzzle0ObjectsHolder;

    private static readonly KeyCode EnterPlatformViewKey = KeyCode.E;
    private static readonly KeyCode ExitPlatformViewKey = KeyCode.Escape;

    private enum GameState
    {
        WorldView,
        BookPlatformView
    }

    public List<Interactable0thPuzzleObject> allBooks;
    public List<Interactable0thPuzzleObject> representativeBooks;

    private GameState currentState = GameState.WorldView;
    private Interactable0thPuzzlePlatform bookPlatform;


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
    }

    private void Update()
    {
        DetectInteractionConditionsMet();
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
        if (bookPlatform.HasPlace()) //A book can be placed on platform with other books
        {
            InteractableBehaviourPutOnBookPlatform putOnBookPlatformBehaviour = new();
            RaiseInteractionConditionsMet(this, interactableInHand, putOnBookPlatformBehaviour);
        }
    }

    private void DetectIfBooksNear_HandsEmpty()
    {

    }

    private void DetectIfBookPlatformNear_HandsEmpty()
    {
        if (bookPlatform.HasBooks()) //Any book can be got into hand when at least one book exists on platform
        {
            DetectPlatformView();
        }
    }

    private void DetectPlatformView()
    {
        //Display UI for the key to be pressed when game switches to platform view
        if (IsEnterPlatformViewKeyPressed())
        {
            //Delete diplay of UI for the key of switching
            SetGameState(GameState.BookPlatformView);
        }
    }

    private bool IsEnterPlatformViewKeyPressed()
    {
        return Input.GetKeyDown(EnterPlatformViewKey);
    }

    private bool IsExitPlatformViewKeyPressed()
    {
        return Input.GetKeyDown(ExitPlatformViewKey);
    }
    
    private void SetGameState(GameState gameState)
    {
        currentState = gameState;
    }

    private void InitializeObjects()
    {
        bookPlatform = Interactable0thPuzzlePlatform.Instance;
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

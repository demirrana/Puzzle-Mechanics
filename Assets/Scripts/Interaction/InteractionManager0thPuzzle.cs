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

    private void Start()
    {
        InitializeObjects();
    }


    public Transform GetObjectsHolderTransform()
    {
        return puzzle0ObjectsHolder;
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

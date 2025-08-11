using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager5thPuzzle : InteractionManager
{
    public static InteractionManager5thPuzzle Instance { get; private set; }

    public event EventHandler<List<Collider>> OnObjectCollidersApproached;
    public event EventHandler OnNoInteractableNear;
    public event EventHandler<Interactable> OnInteractableApproached;

    private enum GameState
    {
        WorldView,
        TableView
    }

    private GameState currentState = GameState.WorldView;

    private void Awake()
    {
        SetInstance();
    }

    protected override void Start()
    {
        base.Start();
        OnObjectCollidersApproached += ObjectCollidersApproached_PlayerInteractionManager;
        OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
    }

    private void Update()
    {
        DetectInteractionConditionsMet();
    }

    protected override void DetectInteractionConditionsMet()
    {
        if (currentState == GameState.WorldView)
        {
            DetectInteractionConditionsMet_WorldView();
        }
        else
        {
            DetectInteractionConditionsMet_TableView();
        }
    }

    protected void DetectInteractionConditionsMet_WorldView()
    {
        if (AreHandsFull())
        {
            RaiseInteractionConditionsMet(this, interactableInHand); //dropping object

            if (IsNear(Interactable5thPuzzleTable.Instance.transform) && Interactable5thPuzzleTable.Instance.HasEmptySlots())
            {
                RaiseInteractionConditionsMet(this, Interactable5thPuzzleTable.Instance);
            }
        }
        else
        {
            DetectAnyColliderApproached();
        }
    }

    protected void DetectInteractionConditionsMet_TableView()
    {
        if (AreHandsFull())
        {
            //object should be dragged around
            DetectEmptySlotsOnTable();
            RaiseInteractionConditionsMet(this, interactableInHand); //exitting table view
        }
        else
        {
            DetectFullSlotsOnTable(); //this should also handle the behaviour of the object inside it (it will be dragged once obtained)
        }
        RaiseInteractionConditionsMet(this, Interactable5thPuzzleTable.Instance); //closing table view
    }

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
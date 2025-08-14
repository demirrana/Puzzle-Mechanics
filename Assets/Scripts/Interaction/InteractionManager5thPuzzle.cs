using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager5thPuzzle : InteractionManager<IInteractableBehaviour5thPuzzle>
{
    public static InteractionManager5thPuzzle Instance { get; private set; }

    public event EventHandler<List<Collider>> OnObjectCollidersApproached;
    public event EventHandler OnNoInteractableNear;
    public event EventHandler<Interactable<IInteractableBehaviour5thPuzzle>> OnInteractableApproached;

    [SerializeField] private Transform puzzle5ObjectsHolder;

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

    public Transform GetObjectsHolderTransform()
    {
        return puzzle5ObjectsHolder;
    }

    protected override void DetectInteractionConditionsMet()
    {
        if (currentState == GameState.WorldView)
        {
            //Debug.Log("Game View is active");
            DetectInteractionConditionsMet_WorldView();
        }
        else
        {
            //Debug.Log("Table View is active");
            DetectInteractionConditionsMet_TableView();
        }
    }

    protected void DetectInteractionConditionsMet_WorldView()
    {
        if (AreHandsFull())
        {
            //Debug.Log("Hands are full");
            RaiseInteractionConditionsMet(this, interactableInHand); //dropping object

            if (IsNear(Interactable5thPuzzleTable.Instance.transform) && Interactable5thPuzzleTable.Instance.HasEmptySlots())
            {
                RaiseInteractionConditionsMet(this, Interactable5thPuzzleTable.Instance);
            }
        }
        else
        {
            //Debug.Log("Hands are empty.");
            DetectAnyColliderApproached();
        }
    }

    protected void DetectInteractionConditionsMet_TableView()
    {
        if (AreHandsFull())
        {
            Debug.Log("Hands are full.");
            //object should be dragged around
            DetectEmptySlotsOnTable();
            RaiseInteractionConditionsMet(this, interactableInHand); //exitting table view
        }
        else
        {
            Debug.Log("Hands are empty.");
            DetectFullSlotsOnTable(); //this should also handle the behaviour of the object inside it (it will be dragged once obtained)
        }
        RaiseInteractionConditionsMet(this, Interactable5thPuzzleTable.Instance); //closing table view
    }

    protected void DetectAnyColliderApproached()
    {
        List<Collider> hitColliders = GetCollidersApproached();

        switch (hitColliders.Count)
        {
            case 0:
                OnNoInteractableNear?.Invoke(this, null);
                break;
            default:
                OnObjectCollidersApproached?.Invoke(this, hitColliders);
                break;
        }
    }

    protected void ObjectCollidersApproached_PlayerInteractionManager(object sender, List<Collider> colliderList)
    {
        List<Interactable<IInteractableBehaviour5thPuzzle>> interactableObjects = GetNearInteractablesList(colliderList);
        DetectInteractableApproached(interactableObjects);
    }

    protected virtual List<Interactable<IInteractableBehaviour5thPuzzle>> GetNearInteractablesList(List<Collider> colliderList)
    {
        List<Interactable<IInteractableBehaviour5thPuzzle>> interactableObjects = new();

        foreach (Collider collider in colliderList)
        {
            if (collider != null)
            {
                GameObject hitObject = collider.gameObject;

                //Making sure the object is an interactable one
                if (hitObject.TryGetComponent<Interactable<IInteractableBehaviour5thPuzzle>>(out var interactableObject))
                {
                    interactableObjects.Add(interactableObject);
                }
            }
        }

        return interactableObjects;
    }

    //Gets the Interactable object near and invokes the event OnInteractableApproached with it
    protected virtual void DetectInteractableApproached(List<Interactable<IInteractableBehaviour5thPuzzle>> interactableObjects)
    {
        switch (interactableObjects.Count)
        {
            case 0:
                break;
            case 1:
                OnInteractableApproached?.Invoke(this, interactableObjects[0]);
                break;
            //TO BE CHANGED IN THE FUTURE
            default:
                Debug.Log("There are more than 1 interactables: " + interactableObjects[0].name + " and " + interactableObjects[1].name);
                //TO BE CHANGED: When there are more than 1 interactables, the camera angle should decide which one to interact with
                OnInteractableApproached?.Invoke(this, interactableObjects[1]); //For now, the one after first interactable can be interacted 
                break;
        }
    }

    protected void RaiseInteractableApproached(object sender, Interactable<IInteractableBehaviour5thPuzzle> interactable)
    {
        OnInteractableApproached?.Invoke(sender, interactable);
    }

    protected void InteractableApproached_PlayerInteractionManager(object sender, Interactable<IInteractableBehaviour5thPuzzle> interactable)
    {
        foreach (IInteractableBehaviour5thPuzzle interactionBehaviour in interactable.GetInteractionBehaviours())
        {
            DetectBehaviourApplied(interactable, interactionBehaviour);
        }
    }

    private void DetectEmptySlotsOnTable()
    {
        if (Interactable5thPuzzleTable.Instance.HasEmptySlots())
        {
            RaiseInteractionConditionsMet(this, Interactable5thPuzzleTable.Instance.GetPointedEmptySlot());
        }
    }

    private void DetectFullSlotsOnTable()
    {
        if (!Interactable5thPuzzleTable.Instance.HasEmptySlots())
        {
            Interactable5thPuzzleTableSlot pointedFullSlot = Interactable5thPuzzleTable.Instance.GetPointedFullSlot();
            RaiseInteractionConditionsMet(this, pointedFullSlot);
            RaiseInteractionConditionsMet(this, pointedFullSlot.GetInteractableOnSlot());
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
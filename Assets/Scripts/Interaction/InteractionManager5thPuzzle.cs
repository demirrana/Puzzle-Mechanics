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
    private Interactable5thPuzzleTableSlot pointedSlotBeforeKeyPress = null;
    private InteractionPanelIndividual InteractionPanelIndividual;
    private Interactable5thPuzzleTable puzzle5Table;

    private void Awake()
    {
        SetInstance();
    }

    protected override void Start()
    {
        base.Start();
        InitializeInstances();
        OnObjectCollidersApproached += ObjectCollidersApproached_PlayerInteractionManager;
        OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
        OnNoInteractableNear += NoInteractableNear_PlayerInteractionManager5thPuzzle;
    }

    private void Update()
    {
        DetectInteractionConditionsMet();
    }

    public Transform GetObjectsHolderTransform()
    {
        return puzzle5ObjectsHolder;
    }

    public void ChangeGameState()
    {
        currentState = currentState == GameState.WorldView ? GameState.TableView : GameState.WorldView;
    }

    public Interactable5thPuzzleTableSlot GetPointedSlotBeforeKeyPress()
    {
        return pointedSlotBeforeKeyPress;
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
            Debug.Log("Hands are full in game view.");
            if (IsNear(puzzle5Table.transform) && puzzle5Table.HasEmptySlots())
            {
                //Debug.Log("Get requested behaviour from list: " + interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourDragOnTableFromHand()).ToString());
                RaiseInteractionConditionsMet(this, interactableInHand, interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourDragOnTableFromHand()));
                InteractionPanelIndividual.RaiseInteractionPanelActivated(this, puzzle5Table.transform);
            }
            if (!IsNear(puzzle5Table.transform))
            {
                InteractionPanelIndividual.RaiseInteractionPanelDeactivated(puzzle5Table);
            }
            if (interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourDropOnFloor()) != null) //behaviour list may have been changed after switching to table view
            {
                RaiseInteractionConditionsMet(this, interactableInHand, interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourDropOnFloor())); //dropping object
            }
        }
        else
        {
            Debug.Log("Hands are empty in game view.");
            DetectAnyColliderApproached();
            //could be simplified
            if (IsNear(puzzle5Table.transform) && puzzle5Table.HasFullSlots())
            {
                puzzle5Table.DetectOpenTableView();
                InteractionPanelIndividual.RaiseInteractionPanelActivated(this, puzzle5Table.transform);
            }
        }
    }

    protected void DetectInteractionConditionsMet_TableView()
    {
        if (AreHandsFull())
        {
            Debug.Log("Hands are full in the table view.");
            MoveInteractableWithMouse(); //object is dragged on table
            RaiseInteractionConditionsMet(this, interactableInHand, interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourPickUpFromTableToHand())); //exitting table view
            DetectEmptySlotsOnTable(); //finds slots to put the object on and raises that event
        }
        else
        {
            Debug.Log("Hands are empty in table view.");
            DetectFullSlotsOnTable(); //this should also handle the behaviour of the object inside it (it will be dragged once obtained)
            puzzle5Table.DetectCloseTableView();
        }
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
                InteractionPanelIndividual.RaiseInteractionPanelActivated(this, interactableObjects[0].transform);
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

    //used to deactivate individual interaction panel
    private void NoInteractableNear_PlayerInteractionManager5thPuzzle(object sender, EventArgs e)
    {
        bool tableNear = IsNear(puzzle5Table.transform);
        bool tableHasFullSlots = puzzle5Table.HasFullSlots();
        if (!tableNear || (tableNear && !tableHasFullSlots))
        {
            InteractionPanelIndividual.RaiseInteractionPanelDeactivated(sender);
        }
    }

    //interaction panel is displayed on the empty slot that is pointed by the mouse
    private void DetectEmptySlotsOnTable()
    {
        if (AreHandsFull() && puzzle5Table.HasEmptySlots()) //hands may be empty if game view is active in the loop
        {
            SetPointedSlotBeforeKeyPress(puzzle5Table.GetPointedEmptySlot());
            InteractionPanelIndividual.RaiseInteractionPanelActivated(this, pointedSlotBeforeKeyPress.transform);
            //Debug.Log("there are empty slots and the pointed one is named: " + pointedEmptySlot.name);
            //Displaying interaction key on the slot tilted appropriately to the table camera
            RaiseInteractionConditionsMet(this, interactableInHand, interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourPutOnTableSlot())); //putting object on slot
        }
    }

    //Displays the interaction key on the slot that is pointed and full. Also raises the event for the object on slot to be picked up.
    private void DetectFullSlotsOnTable()
    {
        if (puzzle5Table.HasFullSlots())
        {
            //Interactable5thPuzzleTable.Instance.LogEmptyAndFullSlots(); //for debugging
            SetPointedSlotBeforeKeyPress(puzzle5Table.GetPointedFullSlot());
            InteractionPanelIndividual.RaiseInteractionPanelActivated(this, pointedSlotBeforeKeyPress.transform);
            Interactable5thPuzzleObject interactableOnSlot = pointedSlotBeforeKeyPress.GetInteractableOnSlot();
            //Debug.Log("there are full slots and the pointed one is named: " + pointedFullSlot.name);
            RaiseInteractionConditionsMet(this, interactableOnSlot, interactableOnSlot.GetRequestedBehaviourFromList(new InteractableBehaviourDragOnTableFromSlot())); //putting object on slot
            //Displaying interaction key on the slot tilted appropriately to the table camera
        }
    }

    private void SetPointedSlotBeforeKeyPress(Interactable5thPuzzleTableSlot slot)
    {
        pointedSlotBeforeKeyPress = slot;
    }

    public Vector3 GetMousePositionInWorld()
    {
        float distanceFromCamera = 1.7f; //TO BE CHANGED according to the real table and camera
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = distanceFromCamera;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        return worldPos;
    }

    private void MoveInteractableWithMouse()
    {
        float lerpSpeed = 10f;
        Vector3 worldPos = GetMousePositionInWorld();
        interactableInHand.transform.position = Vector3.Lerp(interactableInHand.transform.position, worldPos, Time.deltaTime * lerpSpeed);
    }

    private void InitializeInstances()
    { 
        puzzle5Table = Interactable5thPuzzleTable.Instance;
        InteractionPanelIndividual = InteractionPanelIndividual.Instance;
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
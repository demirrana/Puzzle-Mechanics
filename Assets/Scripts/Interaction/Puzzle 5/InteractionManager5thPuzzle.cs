using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager5thPuzzle : InteractionManager<IInteractableBehaviour5thPuzzle>
{
    #region Singleton
    public static InteractionManager5thPuzzle Instance { get; private set; }
    #endregion

    #region Fields
    [SerializeField] private Transform puzzle5ObjectsHolder;

    private enum GameState
    {
        WorldView,
        TableView
    }

    private GameState currentState = GameState.WorldView;
    private Interactable5thPuzzleTableSlot pointedSlotBeforeKeyPress = null;
    private InteractionPanelMovable InteractionPanelIndividual;
    private Interactable5thPuzzleTable puzzle5Table;
    private float dropRadius = 0.5f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        SetInstance();
    }

    protected override void Start()
    {
        base.Start();
        InitializeInstances();
        OnObjectCollidersApproached += PlayerInteractionManager_ObjectCollidersApproached;
        OnInteractableApproached += PlayerInteractionManager_InteractableApproached;
        OnNoInteractableNear += PlayerInteractionManager_NoInteractableNear;
    }

    private void Update()
    {
        DetectInteractionConditionsMet();
    }

    private void OnDestroy()
    {
        OnObjectCollidersApproached -= PlayerInteractionManager_ObjectCollidersApproached;
        OnInteractableApproached -= PlayerInteractionManager_InteractableApproached;
        OnNoInteractableNear -= PlayerInteractionManager_NoInteractableNear;
    }
    #endregion

    #region Public API (Getters & State Control)
    public Transform GetObjectsHolderTransform() => puzzle5ObjectsHolder;

    public Interactable5thPuzzleTableSlot GetPointedSlotBeforeKeyPress()
    {
        //Debug.Log("GetPointedSlotBeforeKeyPress is called. The pointed slot is: " + pointedSlotBeforeKeyPress.name);
        return pointedSlotBeforeKeyPress;
    }

    public float GetDropRadius() => dropRadius;

    public void ChangeGameState()
    {
        currentState = currentState == GameState.WorldView ? GameState.TableView : GameState.WorldView;
    }

    public Vector3 GetNearestPosCollidingWithNothing(float dropRadius)
    {
        float objectRadius = 0.2f;
        int pointCount = 6;
        float angleStep = 360f / pointCount;
        LayerMask layerMask = default;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = transform.position.x + Mathf.Cos(angle) * dropRadius;
            float z = transform.position.z + Mathf.Sin(angle) * dropRadius;
            Vector3 point = new(x, transform.position.y, z);
            if (Physics.OverlapSphere(point, objectRadius, layerMask).Length == 0)
                return point;
        }

        return GetNearestPosCollidingWithNothing(2 * dropRadius);
    }
    #endregion

    #region Main Detection Logic (Core Processors)
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
        bool isTableNear = IsNear(puzzle5Table.transform);
        bool hasTableEmptySlots = puzzle5Table.HasEmptySlots();
        bool hasTableFullSlots = puzzle5Table.HasFullSlots();

        if (AreHandsFull()) //drop obj on floor
        {
            bool isDropOnFloorPossible = interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourDropOnFloor()) != null;
            DetectConditionsMet_WorldViewHandsFull(isTableNear, hasTableEmptySlots, isDropOnFloorPossible);
        }
        else //pick obj from table or pick obj from floor
        {
            DetectConditionsMet_WorldViewHandsEmpty(isTableNear, hasTableFullSlots);
        }
    }

    protected void DetectInteractionConditionsMet_TableView()
    {
        if (AreHandsFull()) //object is dragged on table or put on table slot or exitting table view
        {
            DetectConditionsMet_TableViewHandsFull();
        }
        else //pick obj from table slot or exitting table view
        {
            DetectConditionsMet_TableViewHandsEmpty();
        }
    }

    protected void DetectConditionsMet_WorldViewHandsFull(bool isTableNear, bool hasTableEmptySlots, bool isDropOnFloorPossible)
    {
        //Debug.Log("Hands are full in game view.");
        if (isTableNear && hasTableEmptySlots)
        {
            //Debug.Log("Get requested behaviour from list: " + interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourDragOnTableFromHand()).ToString());
            DetectNearTableForPuttingObject();
        }
        if (!isTableNear)
        {
            InteractionPanelIndividual.RaiseInteractionPanelDeactivated(puzzle5Table);
        }
        if (isDropOnFloorPossible) //behaviour list may have been changed after switching to table view
        {
            RaiseInteractionConditionsMet(this, interactableInHand, interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourDropOnFloor())); //dropping object
        }
    }

    protected void DetectConditionsMet_WorldViewHandsEmpty(bool isTableNear, bool hasTableFullSlots)
    {
        //Debug.Log("Hands are empty in game view.");
        DetectAnyColliderApproached();
        if (isTableNear && hasTableFullSlots)
        {
            DetectNearTableWithObjectsOnIt();
        }
    }

    protected void DetectConditionsMet_TableViewHandsFull()
    {
        //Debug.Log("Hands are full in the table view.");
        MoveInteractableWithMouse(); //object is dragged on table
        RaiseInteractionConditionsMet(this, interactableInHand, interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourPickUpFromTableToHand())); //exitting table view
        DetectEmptySlotsOnTable(); //finds slots to put the object on and raises that event
    }

    protected void DetectConditionsMet_TableViewHandsEmpty()
    {
        //Debug.Log("Hands are empty in table view.");
        DetectFullSlotsOnTable(); //this should also handle the behaviour of the object inside it (it will be dragged once obtained)
        puzzle5Table.DetectCloseTableView();
    }
    #endregion

    #region Private Detection & Helper Methods
    private void DetectNearTableForPuttingObject()
    {
        InteractableBehaviourDragOnTableFromHand dragOnTableFromHand = new();
        RaiseInteractionConditionsMet(this, interactableInHand, interactableInHand.GetRequestedBehaviourFromList(dragOnTableFromHand));
        ActivateInteractionPanel(this, puzzle5Table.transform, dragOnTableFromHand.InteractionKeyCode);
    }

    private void DetectNearTableWithObjectsOnIt()
    {
        KeyCode openTableViewKey = puzzle5Table.GetKeyForOpenTableView();
        puzzle5Table.DetectOpenTableView();
        ActivateInteractionPanel(this, puzzle5Table.transform, openTableViewKey);
    }

    //interaction panel is displayed on the empty slot that is pointed by the mouse
    private void DetectEmptySlotsOnTable()
    {
        //Debug.Log("DetectEmptySlotsOnTable is called.");
        if (AreHandsFull() && puzzle5Table.HasEmptySlots()) //hands may be empty if game view is active in the loop
        {
            //Debug.Log("Hands are full and there are empty slots on the table.");
            SetPointedSlotBeforeKeyPress(puzzle5Table.GetPointedEmptySlot());
            Transform pointedSlotTransform = pointedSlotBeforeKeyPress.transform;
            //Debug.Log("Pointed slot before key press is: " + pointedSlotBeforeKeyPress.name);
            InteractableBehaviourPutOnTableSlot putOnSlot = new();
            ActivateInteractionPanel(this, pointedSlotTransform, putOnSlot.InteractionKeyCode);
            //Debug.Log("there are empty slots and the pointed one is named: " + pointedEmptySlot.name);
            RaiseInteractionConditionsMet(this, interactableInHand, interactableInHand.GetRequestedBehaviourFromList(putOnSlot)); //putting object on slot
        }
    }

    //Displays the interaction key on the slot that is pointed and full. Also raises the event for the object on slot to be picked up.
    private void DetectFullSlotsOnTable()
    {
        if (puzzle5Table.HasFullSlots())
        {
            SetPointedSlotBeforeKeyPress(puzzle5Table.GetPointedFullSlot());
            Transform pointedSlotTransform = pointedSlotBeforeKeyPress.transform;
            InteractableBehaviourDragOnTableFromSlot pickUpFromSlot = new();
            //Interactable5thPuzzleTable.Instance.LogEmptyAndFullSlots(); //for debugging
            ActivateInteractionPanel(this, pointedSlotTransform, pickUpFromSlot.InteractionKeyCode);
            
            Interactable5thPuzzleObject interactableOnSlot = pointedSlotBeforeKeyPress.GetInteractableOnSlot();
            //Debug.Log("there are full slots and the pointed one is named: " + pointedFullSlot.name);
            RaiseInteractionConditionsMet(this, interactableOnSlot, interactableOnSlot.GetRequestedBehaviourFromList(pickUpFromSlot)); //putting object on slot
        }
    }

    private void SetPointedSlotBeforeKeyPress(Interactable5thPuzzleTableSlot slot)
    {
        pointedSlotBeforeKeyPress = slot;
    }

    private void MoveInteractableWithMouse()
    {
        float lerpSpeed = 10f;
        Vector3 worldPos = MouseManager.Instance.GetMousePositionInWorld();
        interactableInHand.transform.position = Vector3.Lerp(interactableInHand.transform.position, worldPos, Time.deltaTime * lerpSpeed);
    }

    private void ActivateInteractionPanel(object sender, Transform targetTransform, KeyCode interactionKey)
    {
        InteractionPanelIndividual.RaiseInteractionPanelActivated(sender, targetTransform, interactionKey);
    }

    private void InitializeInstances()
    { 
        puzzle5Table = Interactable5thPuzzleTable.Instance;
        InteractionPanelIndividual = InteractionPanelMovable.Instance;
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

    #region Base Class Overrides (Event Responses)
    protected override void PlayerInteractionManager_InteractableApproached(object sender, Interactable<IInteractableBehaviour5thPuzzle> interactable)
    {
        InteractableBehaviourPickUpFromFloor pickUpFromFloor = new(); //TO BE CHANGED LATER
        ActivateInteractionPanel(this, interactable.transform, pickUpFromFloor.InteractionKeyCode);

        foreach (IInteractableBehaviour5thPuzzle interactionBehaviour in interactable.GetInteractionBehaviours())
        {
            DetectBehaviourApplied(interactable, interactionBehaviour);
        }
    }

    //used to deactivate individual interaction panel
    protected override void PlayerInteractionManager_NoInteractableNear(object sender, EventArgs e)
    {
        bool tableNear = IsNear(puzzle5Table.transform);
        bool tableHasFullSlots = puzzle5Table.HasFullSlots();
        if (!tableNear || (tableNear && !tableHasFullSlots))
        {
            InteractionPanelIndividual.RaiseInteractionPanelDeactivated(sender);
        }
    }
    #endregion
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager5thPuzzle : InteractionManager<IInteractableBehaviour5thPuzzle>
{
    public static InteractionManager5thPuzzle Instance { get; private set; }

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
    private float dropRadius = 0.5f;

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

    public float GetDropRadius()
    {
        return dropRadius;
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

        if (AreHandsFull())
        {
            bool isDropOnFloorPossible = interactableInHand.GetRequestedBehaviourFromList(new InteractableBehaviourDropOnFloor()) != null;
            DetectConditionsMet_WorldViewHandsFull(isTableNear, hasTableEmptySlots, isDropOnFloorPossible);
        }
        else
        {
            DetectConditionsMet_WorldViewHandsEmpty(isTableNear, hasTableFullSlots);
        }
    }

    protected void DetectInteractionConditionsMet_TableView()
    {
        if (AreHandsFull())
        {
            DetectConditionsMet_TableViewHandsFull();
        }
        else
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

    protected void DetectAnyColliderApproached()
    {
        List<Collider> hitColliders = GetCollidersApproached();

        switch (hitColliders.Count)
        {
            case 0:
                RaiseNoInteractableNear(this);
                break;
            default:
                RaiseObjectCollidersApproached(this, hitColliders);
                break;
        }
    }

    protected void ObjectCollidersApproached_PlayerInteractionManager(object sender, List<Collider> colliderList)
    {
        List<Interactable<IInteractableBehaviour5thPuzzle>> interactableObjects = GetNearInteractablesList(colliderList);
        DetectInteractableApproached(interactableObjects);
    }

    //Gets the Interactable object near and invokes the event OnInteractableApproached with it
    protected virtual void DetectInteractableApproached(List<Interactable<IInteractableBehaviour5thPuzzle>> interactableObjects)
    {
        switch (interactableObjects.Count)
        {
            case 0:
                break;
            case 1:
                Interactable<IInteractableBehaviour5thPuzzle> interactable = interactableObjects[0];
                InteractableBehaviourPickUpFromFloor pickUpFromFloor = new();
                ActivateInteractionPanel(this, interactable.transform, pickUpFromFloor.InteractionKeyCode);
                RaiseInteractableApproached(this, interactableObjects[0]);
                break;
            //TO BE CHANGED IN THE FUTURE
            default:
                Debug.Log("There are more than 1 interactables: " + interactableObjects[0].name + " and " + interactableObjects[1].name);
                //TO BE CHANGED: When there are more than 1 interactables, the camera angle should decide which one to interact with
                RaiseInteractableApproached(this, interactableObjects[1]); //For now, the one after first interactable can be interacted 
                break;
        }
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
            Transform pointedSlotTransform = pointedSlotBeforeKeyPress.transform;
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

    private void ActivateInteractionPanel(object sender, Transform targetTransform, KeyCode interactionKey)
    {
        InteractionPanelIndividual.RaiseInteractionPanelActivated(sender, targetTransform, interactionKey);
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
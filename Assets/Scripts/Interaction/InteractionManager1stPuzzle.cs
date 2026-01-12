using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class InteractionManager1stPuzzle : InteractionManager<IInteractableBehaviour1stPuzzle> //to be changed into 1stPuzzle
{
    public static InteractionManager1stPuzzle Instance { get; private set; }

    public event EventHandler<Interactable1stPuzzleObject> OnAnyKeyPartCollected;
    public event EventHandler OnEditViewActivated;
    public event EventHandler OnEditViewDeactivated;
    public event EventHandler<Interactable1stPuzzleDoor> OnDoorNear;
    public event EventHandler OnKeyDeselected;

    public enum ViewMode
    {
        WorldView,
        EditView
    }

    [SerializeField] private Interactable<IInteractableBehaviour1stPuzzle> mainKeyPartPrefab;
    [SerializeField] private Transform previewTransform;
    [SerializeField] private Transform keysInitialParent;
    [SerializeField] private List<int> totalCorrectSnapCounts;
    [SerializeField] private int doorCount;

    private bool isPuzzleCompleted;

    private ViewMode currentViewMode;

    private List<Interactable<IInteractableBehaviour1stPuzzle>> collectedKeyParts;

    private List<Interactable1stPuzzleObject> combinedKeys;

    private int currentDoorIndex;
    private int correctSnapCount;

    private List<GameObject> keyPartsPreviewed;
    private bool hasPreviewStarted;
    private GameObject previewingKeyPartObject;

    public void RegisterCombinedKey(Interactable1stPuzzleObject newKey) 
    {
        combinedKeys.Add(newKey);
    }

    public void UnregisterCombinedKey(Interactable1stPuzzleObject newKey)
    {
        combinedKeys.Remove(newKey);
    }

    public Interactable1stPuzzleObject GetKeyInCombinedKeys(GameObject obj) //controls if it is main key or any key that is attached to it
    {
        Interactable1stPuzzleObject key = obj.GetComponentInParent<Interactable1stPuzzleObject>();

        if (key != null)
            return combinedKeys.Find(x => x == key);

        return null;
    }

    public Transform GetKeysInitialParent()
    {
        return keysInitialParent;
    }

    private void Awake()
    {
        SetInstance();
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        InitializeVariables();
        SubscribeEvents();
    }

    private void InitializeVariables()
    {
        currentViewMode = ViewMode.WorldView;
        collectedKeyParts = new();
        combinedKeys = new();
        keyPartsPreviewed = new();
    }

    private void SubscribeEvents()
    {
        OnObjectCollidersApproached += PlayerInteractionManager_ObjectCollidersApproached;
        OnInteractableApproached += PlayerInteractionManager_InteractableApproached;
        OnNoInteractableNear += PlayerInteractionManager_NoInteractableNear;
        SnapHandler.Instance.OnKeySnappedToSocket += InteractionManager1stPuzzle_KeySnappedToSocket;
        SnapHandler.Instance.OnKeyUnsnappedFromSocket += InteractionManager1stPuzzle_KeyUnsnappedFromSocket;
        UI_Puzzle1Manager.Instance.OnHoveredInventorySlotChanged += InteractionManager1stPuzzle_HoveredInventorySlotChanged;
    }

    private void Update()
    {
        DetectTakeMainPartInHand();
        DetectInteractionConditionsMet();
        DetectProceedToNextDoor();
    }

    private void DetectTakeMainPartInHand() //this will happen when that scene's some exact part is finished SO EDIT LATER
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TakeMainKeyPartInHand();
        }
    }

    private void TakeMainKeyPartInHand() //to be called when the previous scene (petting a cat scene) finishes
    {
        interactableInHand = Instantiate(
            mainKeyPartPrefab,
            GetHandPosition(),
            GetHandTransform().rotation,
            GetHandTransform()
        );

        combinedKeys.Add(interactableInHand.GetComponent<Interactable1stPuzzleObject>());

        RaiseInteractableInHandChanged(interactableInHand);
    }

    protected override void DetectInteractionConditionsMet()
    {
        if (currentViewMode == ViewMode.WorldView)
        {
            DetectAnyColliderApproached();
        }
        else if (currentViewMode == ViewMode.EditView)
        {
            DetectInteractionInEditView();
        }
    }

    private void DetectInteractionInEditView()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //exit from edit view
        {
            ExitEditView();
            return;
        }

        UI_Puzzle1Manager.Instance.HandleSlotInteraction();

        RotatePreviewCameraAroundKeyPart();
        MoveKeyPartOnScreen();

        //show chosen key part on screen along with the inventory view
        //display the main key on screen
        //make background blurry
    }

    private void ExitEditView()
    {
        currentViewMode = ViewMode.WorldView;
        OnEditViewDeactivated?.Invoke(this, EventArgs.Empty);
        ResetMainKeyRotation();
        CameraManager.Instance.SwitchToNextCamera();
    }

    private void ResetMainKeyRotation()
    {
        Interactable1stPuzzleMainKey mainKey = Interactable1stPuzzleMainKey.Instance;
        mainKey.SetRotation(mainKey.GetInitialRotation());
    }

    private void RotatePreviewCameraAroundKeyPart()
    {
        if (hasPreviewStarted)
        {
            PreviewCamera.Instance.RotateAroundObject(previewingKeyPartObject);
        }
    }

    private void MoveKeyPartOnScreen()
    {
        if (SnapHandler.Instance.IsKeyFollowingMouse())
        {
            SnapHandler.Instance.SnapKeyPlugToSocket();
            DeselectKeyPart();
        }
        else
        {
            RotateMainKey();
            SnapHandler.Instance.UnsnapKeyFromSocket();
        }
    }

    private void RotateMainKey()
    {
        if (!UI_Puzzle1Manager.Instance.IsMouseOverInventoryPanel())
        {
            Interactable1stPuzzleMainKey.Instance.RotateObject();
        }
    }

    private void DeselectKeyPart()
    {
        if (Input.GetMouseButtonDown(1)) //deselect when rmb is clicked
        {
            OnKeyDeselected?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DetectProceedToNextDoor()
    {
        if (correctSnapCount == totalCorrectSnapCounts[currentDoorIndex])
        {
            currentDoorIndex++;

            if (currentDoorIndex != doorCount)
                RecountCorrectSnaps();
            else
                isPuzzleCompleted = true;
        }
    }

    private void RecountCorrectSnaps()
    {
        correctSnapCount = 0;
        foreach (Interactable1stPuzzleObject key in combinedKeys)
        {
            foreach (SocketData socket in key.sockets)
            {
                if (socket.isOccupied)
                {
                    if (socket.targetPlugIDs[currentDoorIndex].Equals(socket.snappedKey.GetPlugID()))
                    {
                        correctSnapCount++;
                    }
                }
            }
        }
    }

    protected override void PlayerInteractionManager_ObjectCollidersApproached(object sender, List<Collider> colliderList)
    {
        Interactable1stPuzzleDoor nearDoor = GetTypeNear<Interactable1stPuzzleDoor>();

        if (nearDoor != null) //when a door is near, it is either editing key parts or trying the key in hand on the door
        {
            OnDoorNear?.Invoke(sender, nearDoor);
            //display UI of (try on door, edit key)
            if (Input.GetKeyDown(KeyCode.Alpha1)) //try on door
            {
                
            }
            else if (Input.GetKeyDown(nearDoor.GetInteractionKey())) //edit key
            {
                SwitchToEditView();
            }
        }
        else
        {
            base.PlayerInteractionManager_ObjectCollidersApproached(sender, colliderList);
        }
    }

    private void SwitchToEditView()
    {
        currentViewMode = ViewMode.EditView;
        //update camera view to focus on the main part and freeze the camera
        CameraManager.Instance.FollowWithCamera(interactableInHand.transform, CameraManager.CameraName.GameplayCamera);
        CameraManager.Instance.SwitchToNextCamera();
        //stop showing player (make main key's parent null and update the pos to the mouse pos)
        OnEditViewActivated?.Invoke(this, EventArgs.Empty);
    }

    //approached object could be any interactable in puzzle 1 (such as table, cat etc.)
    //maybe type should be changed into Interactable1stPuzzleObject
    protected override void PlayerInteractionManager_InteractableApproached(object sender, Interactable<IInteractableBehaviour1stPuzzle> keyPart)
    {
        InteractableBehaviourCollectKeyPart collectKeyPart = new();
        RaiseInteractionConditionsMet(sender, keyPart, collectKeyPart);

        /*
        //This is applied in manager of 5th puzzle
        foreach (IInteractableBehaviour1stPuzzle interactionBehaviour in keyPart.GetInteractionBehaviours())
        {
            DetectBehaviourApplied(keyPart, interactionBehaviour);
        }
        */
    }

    protected override void InteractionManager_InteractableInteracted(object sender, InteractionBehaviourEventArgs e)
    {
        base.InteractionManager_InteractableInteracted(sender, e);

        //there are more than 1 types of interactables (door and key part etc.) so each type should be judged differently
        //but for now, this code is based only on key parts

        Interactable<IInteractableBehaviour1stPuzzle> keyPart = e.InteractedObject; //type can be changed

        if (keyPart is Interactable1stPuzzleObject)
        {
            Interactable1stPuzzleObject keyPartObj = keyPart as Interactable1stPuzzleObject;
            OnAnyKeyPartCollected?.Invoke(sender, keyPartObj);

            collectedKeyParts.Add(keyPart);
            keyPartObj.gameObject.Hide();
        }
    }

    private void InteractionManager1stPuzzle_HoveredInventorySlotChanged(object sender, UI_Puzzle1Manager.ChangeInHoveredObjectEventArgs e)
    {
        UI_Puzzle1stObject previousSlot = e.PreviousInventorySlot;
        UI_Puzzle1stObject currentSlot = e.CurrentInventorySlot;

        //Hide all previously previewed keys
        foreach (GameObject keyPart in keyPartsPreviewed)
        {
            keyPart.Hide();
        }
        //find it from keyPartsDisplayed and stop displaying it by hiding it

        HandlePreview(currentSlot.GetKeyPart());
    }

    private void HandlePreview(Interactable1stPuzzleObject keyPart)
    {
        GameObject displayedKeyPartGameObject = FindPreviewedGameObject(keyPart);
        if (displayedKeyPartGameObject == null) //instantiate since it is displayed for the first time
        {
            displayedKeyPartGameObject = Instantiate(keyPart.gameObject, previewTransform); //rotation could be added as parameter
            displayedKeyPartGameObject.transform.localPosition = Vector3.zero;
            keyPartsPreviewed.Add(displayedKeyPartGameObject);
        }

        AdjustVariablesForPreview(displayedKeyPartGameObject);
    }

    private GameObject FindPreviewedGameObject(Interactable1stPuzzleObject keyPart)
    {
        foreach (GameObject displayedGameObject in keyPartsPreviewed)
        {
            Interactable1stPuzzleObject displayedKeyPart = displayedGameObject.GetComponent<Interactable1stPuzzleObject>();
            if (displayedKeyPart.GetKeyPartData() == keyPart.GetKeyPartData())
            {
                return displayedGameObject;
            }
        }

        return null;
    }

    private void AdjustVariablesForPreview(GameObject gameObject)
    {
        gameObject.Show();
        previewingKeyPartObject = gameObject;
        PreviewCamera.Instance.ResetPositionRotation();
        hasPreviewStarted = true;
        //camera shows the object
    }

    private void EndPuzzle()
    {
        //set the follow of GameplayCamera to none
    }
}
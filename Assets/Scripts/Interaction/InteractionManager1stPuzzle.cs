using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class InteractionManager1stPuzzle : InteractionManager<IInteractableBehaviour1stPuzzle> //to be changed into 1stPuzzle
{
    public static InteractionManager1stPuzzle Instance { get; private set; }

    public event EventHandler<Interactable1stPuzzleObject> OnAnyKeyPartCollected;
    public event EventHandler OnEditViewActivated;
    public event EventHandler OnEditViewDeactivated;
    public event EventHandler<Interactable1stPuzzleDoor> OnDoorNear;

    public enum ViewMode
    {
        WorldView,
        EditView
    }

    [SerializeField] private Interactable<IInteractableBehaviour1stPuzzle> mainKeyPartPrefab;
    [SerializeField] private Transform previewTransform;

    private InteractionPanelIndividual InteractionPanelIndividual;
    private ViewMode currentViewMode;

    private List<Interactable<IInteractableBehaviour1stPuzzle>> collectedKeyParts;

    private bool isKeyFollowingMouse;
    private Interactable1stPuzzleObject movingKeyPart;

    private List<GameObject> keyPartsPreviewed;
    private bool hasPreviewStarted;
    private GameObject previewingKeyPartObject;

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
        InteractionPanelIndividual = InteractionPanelIndividual.Instance;
        currentViewMode = ViewMode.WorldView;
        collectedKeyParts = new();
        isKeyFollowingMouse = false;
        movingKeyPart = null;
        keyPartsPreviewed = new();
        hasPreviewStarted = false;
        previewingKeyPartObject = null;
    }

    private void SubscribeEvents()
    {
        OnObjectCollidersApproached += PlayerInteractionManager_ObjectCollidersApproached;
        OnInteractableApproached += PlayerInteractionManager_InteractableApproached;
        OnNoInteractableNear += PlayerInteractionManager_NoInteractableNear;
        UI_Puzzle1Manager.Instance.OnHoveredInventorySlotChanged += InteractionManager1stPuzzle_HoveredInventorySlotChanged;
        UI_Puzzle1Manager.Instance.OnInventorySlotClicked += InteractionManager1stPuzzle_InventorySlotClicked;
    }

    private void Update()
    {
        DetectTakeMainPartInHand();
        DetectInteractionConditionsMet();
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
        RotateMainKey();
        DeselectKeyPart();

        //show chosen key part on screen along with the inventory view
        //display the main key on screen
        //make background blurry
    }

    private void ExitEditView()
    {
        currentViewMode = ViewMode.WorldView;
        OnEditViewDeactivated?.Invoke(this, EventArgs.Empty);
        ResetEditViewRelatedElements();
        ResetMainKeyRotation();
        CameraManager.Instance.SwitchToNextCamera();
    }

    private void ResetEditViewRelatedElements()
    {
        if (movingKeyPart != null)
        {
            movingKeyPart.gameObject.Hide();
            movingKeyPart = null;
        }

        isKeyFollowingMouse = false;
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
        if (isKeyFollowingMouse)
        {
            MoveKeyPartWithMouse(movingKeyPart);
        }
    }

    private void RotateMainKey()
    {
        if (!UI_Puzzle1Manager.Instance.IsMouseOverInventoryPanel() && movingKeyPart == null)
        {
            Interactable1stPuzzleMainKey.Instance.RotateObject();
        }
    }

    private void DeselectKeyPart()
    {
        if (movingKeyPart != null && Input.GetMouseButtonDown(1)) //deselect when rmb is clicked
        {
            movingKeyPart.gameObject.Hide();
            movingKeyPart = null;
            isKeyFollowingMouse = false;
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

        if (previousSlot == null) //Hide all previously previewed keys
        {
            foreach (GameObject keyPart in keyPartsPreviewed)
            {
                keyPart.SetActive(false);
            }
            //find it from keyPartsDisplayed and stop displaying it by hiding it
        }

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

    //feat: add method handling preview
    //-Instantiate the key that is previewed for the first time
    //-Adjust variables to prepare for preview

    private void AdjustVariablesForPreview(GameObject gameObject)
    {
        gameObject.SetActive(true);
        previewingKeyPartObject = gameObject;
        PreviewCamera.Instance.ResetPositionRotation();
        hasPreviewStarted = true;
        //camera shows the object
    }

    private void InteractionManager1stPuzzle_InventorySlotClicked(object sender, UI_Puzzle1stObject inventorySlot)
    {
        if (movingKeyPart != null && movingKeyPart != inventorySlot.GetKeyPart()) //stop displaying the previously chosen key part
        {
            movingKeyPart.gameObject.Hide();
        }

        movingKeyPart = inventorySlot.GetKeyPart();
        movingKeyPart.gameObject.Show();
        isKeyFollowingMouse = true;
    }

    private void EndPuzzle()
    {
        //set the follow of GameplayCamera to none
    }

    //The 2 methods below belong to the manager 5th class. These should be placed in MouseManager
    public Vector3 GetMousePositionInWorld()
    {
        float distanceFromCamera = GameplayCamera.Instance.GetComponent<CinemachineFollow>().FollowOffset.z;
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -distanceFromCamera;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        return worldPos;
    }

    private void MoveKeyPartWithMouse(Interactable1stPuzzleObject keyPart)
    {
        float lerpSpeed = 10f;
        Vector3 worldPos = GetMousePositionInWorld();
        keyPart.transform.position = Vector3.Lerp(keyPart.transform.position, worldPos, Time.deltaTime * lerpSpeed);
    }
}
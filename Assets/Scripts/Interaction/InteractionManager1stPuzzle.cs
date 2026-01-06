using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
        if (!UI_Puzzle1Manager.Instance.IsMouseOverInventoryPanel())
        {
            Interactable1stPuzzleMainKey.Instance.RotateObject();
        }
    }

    private void DeselectKeyPart()
    {
        if (Input.GetMouseButtonDown(1)) //deselect when lmb is clicked
        {
            movingKeyPart.gameObject.Hide();
            isKeyFollowingMouse = false;
        }
    }

    protected override void PlayerInteractionManager_ObjectCollidersApproached(object sender, List<Collider> colliderList)
    {
        Interactable1stPuzzleDoor nearDoor = GetTypeNear<Interactable1stPuzzleDoor>();

        if (nearDoor != null) //when a door is near, it is either editing key parts or trying the key in hand on the door
        {
            OnDoorNear?.Invoke(sender, nearDoor);
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
        CameraManager.Instance.FollowWithCamera(interactableInHand.transform, CameraManager.CameraName.GameplayCamera);
        CameraManager.Instance.SwitchToNextCamera();
        OnEditViewActivated?.Invoke(this, EventArgs.Empty);
    }
    protected override void PlayerInteractionManager_InteractableApproached(object sender, Interactable<IInteractableBehaviour1stPuzzle> keyPart)
    {
        InteractableBehaviourCollectKeyPart collectKeyPart = new();
        RaiseInteractionConditionsMet(sender, keyPart, collectKeyPart);
    protected override void InteractionManager_InteractableInteracted(object sender, InteractionBehaviourEventArgs e)
    {
        base.InteractionManager_InteractableInteracted(sender, e);

        Interactable<IInteractableBehaviour1stPuzzle> keyPart = e.InteractedObject; //type can be changed

        if (keyPart is Interactable1stPuzzleObject)
        {
            Interactable1stPuzzleObject keyPartObj = keyPart as Interactable1stPuzzleObject;
            OnAnyKeyPartCollected?.Invoke(sender, keyPartObj);

            collectedKeyParts.Add(keyPart);
            keyPartObj.gameObject.Hide();
        }
    }
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
}
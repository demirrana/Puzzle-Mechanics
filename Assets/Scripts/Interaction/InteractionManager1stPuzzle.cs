using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class InteractionManager1stPuzzle : InteractionManager<IInteractableBehaviour1stPuzzle> //to be changed into 1stPuzzle
{
    public static InteractionManager1stPuzzle Instance { get; private set; }

    public class SnapToSocketEventArgs : EventArgs
    {
        public SocketData Socket { get; }
        public Interactable1stPuzzleObject SnappedKey { get; }

        public SnapToSocketEventArgs(SocketData socket, Interactable1stPuzzleObject key)
        {
            Socket = socket;
            SnappedKey = key;
        }
    }

    public event EventHandler<Interactable1stPuzzleObject> OnAnyKeyPartCollected;
    public event EventHandler OnEditViewActivated;
    public event EventHandler OnEditViewDeactivated;
    public event EventHandler<Interactable1stPuzzleDoor> OnDoorNear;
    public event EventHandler<SnapToSocketEventArgs> OnKeySnappedToSocket;
    public event EventHandler<SnapToSocketEventArgs> OnSnappedSocketChanged;
    public event EventHandler<SnapToSocketEventArgs> OnKeyUnsnappedFromSocket;
    public event EventHandler OnKeyDeselected;

    public enum ViewMode
    {
        WorldView,
        EditView
    }

    [SerializeField] private Interactable<IInteractableBehaviour1stPuzzle> mainKeyPartPrefab;
    [SerializeField] private Transform previewTransform;
    [SerializeField] private Transform keysInitialParent;

    private InteractionPanelIndividual InteractionPanelIndividual;
    private ViewMode currentViewMode;

    private List<Interactable<IInteractableBehaviour1stPuzzle>> collectedKeyParts;

    private List<Interactable1stPuzzleObject> combinedKeys;

    private bool isKeyFollowingMouse;
    private Interactable1stPuzzleObject movingKeyPart;
    private SocketData currentSnappedSocket;

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
        combinedKeys = new();
        isKeyFollowingMouse = false;
        movingKeyPart = null;
        currentSnappedSocket = null;
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
            SnapKeyPlugToSocket();
        }
        else
        {
            UnsnapKeyFromSocket();
        }
    }

    private void SnapKeyPlugToSocket()
    {
        String plugID = movingKeyPart.GetPlugID();
        Transform plugTransform = movingKeyPart.GetPlugTransform();

        SocketData closestSocket = GetClosestSocket();
        
        if (closestSocket == null)
        {
            if (currentSnappedSocket != null)
            {
                OnSnappedSocketChanged?.Invoke(this, new(closestSocket, movingKeyPart));

                movingKeyPart.SetParent(keysInitialParent);
                movingKeyPart.transform.localRotation = Quaternion.identity;
            }
            
            currentSnappedSocket = closestSocket;
            MoveKeyPartWithMouse(movingKeyPart);
            return;
        }

        if (currentSnappedSocket != closestSocket)
        {
            OnSnappedSocketChanged?.Invoke(this, new(closestSocket, movingKeyPart));
            SnapToNewSocket(closestSocket);
            currentSnappedSocket = closestSocket;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnKeySnappedToSocket?.Invoke(this, new(closestSocket, movingKeyPart));

            closestSocket.isOccupied = true;
            closestSocket.snappedKey = movingKeyPart;
            combinedKeys.Add(movingKeyPart);
            movingKeyPart = null;
            isKeyFollowingMouse = false;
        }
    }

    private SocketData GetClosestSocket()
    {
        int ignoreRaycastLayerIndex = 2;
        movingKeyPart.SetLayer(ignoreRaycastLayerIndex);

        Vector3 hitPoint = Vector3.zero;
        Interactable1stPuzzleObject keyPointedAt = GetKeyPointedAt(out hitPoint);
        movingKeyPart.SetLayer(movingKeyPart.GetInitialLayerIndex());

        if (keyPointedAt != null)
        {
            SocketData closestSocket = keyPointedAt.sockets.OrderBy(x => Vector3.Distance(x.socketTransform.position, hitPoint)).FirstOrDefault();
                
            if (closestSocket != null && !closestSocket.isOccupied)
            {
                return closestSocket;
            }
        }

        return null;
    }

    private Interactable1stPuzzleObject GetKeyPointedAt(out Vector3 hitPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //later, exclude the one held on mouse
        RaycastHit hit;
        hitPoint = Vector3.zero;
        float maxRayDistance = 20f;

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            Interactable1stPuzzleObject hitKey = GetKeyInCombinedKeys(hit.collider.gameObject);

            hitPoint = hit.point;
            return hitKey;
        }

        return null;
    }

    private Interactable1stPuzzleObject GetKeyInCombinedKeys(GameObject obj) //controls if it is main key or any key that is attached to it
    {
        Interactable1stPuzzleObject key = obj.GetComponentInParent<Interactable1stPuzzleObject>();

        if (key != null)
            return combinedKeys.Find(x => x == key);

        return null;
    }

    private void SnapToNewSocket(SocketData newSocket) //snap key visually onto that socket
    {
        movingKeyPart.SetParent(newSocket.socketTransform);
        movingKeyPart.transform.localRotation = newSocket.targetRotation;
        movingKeyPart.transform.localPosition = Vector3.zero;
        movingKeyPart.transform.position += movingKeyPart.transform.position - movingKeyPart.GetPlugTransform().transform.position;
    }

    private void UnsnapKeyFromSocket()
    {
        Interactable1stPuzzleObject keyPointedAt = GetKeyPointedAt(out _);

        if (keyPointedAt == null || keyPointedAt.transform.TryGetComponent<Interactable1stPuzzleMainKey>(out _)) //disregard main key
        {
            InteractionPanelIndividual.Instance.RaiseInteractionPanelDeactivated(this);
            return;
        }

        InteractableBehaviourUnsnapFromSocket unsnap = new();
        KeyCode unsnapKey = unsnap.InteractionKeyCode;

        //show keycode on the key that is about to be unsnapped (keyPointedAt)
        InteractionPanelIndividual.Instance.RaiseInteractionPanelActivated(this, keyPointedAt.transform, unsnapKey);
        //this stays opened on wrong places

        if (Input.GetKeyDown(unsnapKey))
        {
            foreach (SocketData socket in keyPointedAt.sockets) //able to unsnap only the outermost ones
            {
                if (socket.isOccupied)
                {
                    //warn there is a key snapped onto it
                    return;
                }
            }

            //find socket that key is snapped onto and update socket state
            movingKeyPart = keyPointedAt;
            Transform socketTransform = movingKeyPart.transform.parent;
            Transform parentKeyTransform = socketTransform.parent;
            Interactable1stPuzzleObject parentKey = parentKeyTransform.GetComponent<Interactable1stPuzzleObject>();
            SocketData snappedSocket = parentKey.sockets.Find(x => x.socketTransform == socketTransform);
            snappedSocket.isOccupied = false;
            snappedSocket.snappedKey = null;

            OnKeyUnsnappedFromSocket?.Invoke(this, new(snappedSocket, movingKeyPart));

            movingKeyPart.SetParent(keysInitialParent);
            movingKeyPart.transform.localRotation = Quaternion.identity;
            isKeyFollowingMouse = true;
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
            OnKeyDeselected?.Invoke(this, EventArgs.Empty);
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

    //feat: add method handling preview
    //-Instantiate the key that is previewed for the first time
    //-Adjust variables to prepare for preview

    private void AdjustVariablesForPreview(GameObject gameObject)
    {
        gameObject.Show();
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
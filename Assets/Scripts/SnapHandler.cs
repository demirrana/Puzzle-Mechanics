using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class SnapHandler : MonoBehaviour
{
    public static SnapHandler Instance { get; private set; }

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
    
    public event EventHandler<SnapToSocketEventArgs> OnKeySnappedToSocket;
    public event EventHandler<SnapToSocketEventArgs> OnSnappedSocketChanged;
    public event EventHandler<SnapToSocketEventArgs> OnKeyUnsnappedFromSocket;
    public event EventHandler <Interactable1stPuzzleObject> OnKeyPointedAtChanged;

    private bool isKeyFollowingMouse;
    private Interactable1stPuzzleObject movingKeyPart;
    private SocketData currentSnappedSocket;
    private Interactable1stPuzzleObject hoveredKey;

    public bool IsKeyFollowingMouse()
    {
        return isKeyFollowingMouse;
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

    private void Start()
    {
        InteractionManager1stPuzzle.Instance.OnEditViewDeactivated += SnapHandler_EditViewDeactivated;
        InteractionManager1stPuzzle.Instance.OnKeyDeselected += SnapHandler_KeyDeselected;
        UI_Puzzle1Manager.Instance.OnInventorySlotClicked += SnapHandler_InventorySlotClicked;
    }

    public void SnapKeyPlugToSocket()
    {
        String plugID = movingKeyPart.GetPlugID();
        Transform plugTransform = movingKeyPart.GetPlugTransform();

        SocketData closestSocket = GetClosestSocket();
        
        if (closestSocket == null)
        {
            if (currentSnappedSocket != null)
            {
                OnSnappedSocketChanged?.Invoke(this, new(closestSocket, movingKeyPart));

                movingKeyPart.SetParent(InteractionManager1stPuzzle.Instance.GetKeysInitialParent());
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
            movingKeyPart.Snap(closestSocket);
            currentSnappedSocket = closestSocket;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnKeySnappedToSocket?.Invoke(this, new(closestSocket, movingKeyPart));

            closestSocket.isOccupied = true;
            closestSocket.snappedKey = movingKeyPart;
            InteractionManager1stPuzzle.Instance.RegisterCombinedKey(movingKeyPart);
            movingKeyPart = null;
            isKeyFollowingMouse = false;
        }
    }

    public SocketData GetClosestSocket()
    {
        int ignoreRaycastLayerIndex = 2;
        movingKeyPart.SetLayer(ignoreRaycastLayerIndex);

        Vector3 hitPoint = Vector3.zero;
        Interactable1stPuzzleObject keyPointedAt = GetKeyPointedAt(out hitPoint);
        hoveredKey = keyPointedAt;
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
            Interactable1stPuzzleObject hitKey = InteractionManager1stPuzzle.Instance.GetKeyInCombinedKeys(hit.collider.gameObject);

            hitPoint = hit.point;
            return hitKey;
        }

        return null;
    }

    private void MoveKeyPartWithMouse(Interactable1stPuzzleObject keyPart)
    {
        float lerpSpeed = 10f;
        float distanceFromCamera = GameplayCamera.Instance.GetComponent<CinemachineFollow>().FollowOffset.z;
        Vector3 worldPos = MouseManager.Instance.GetMousePositionInWorld(distanceFromCamera);
        keyPart.transform.position = Vector3.Lerp(keyPart.transform.position, worldPos, Time.deltaTime * lerpSpeed);
    }

    private void SnapToNewSocket(SocketData newSocket) //snap key visually onto that socket
    {
        movingKeyPart.SetParent(newSocket.socketTransform);
        movingKeyPart.transform.localRotation = newSocket.targetRotation;
        movingKeyPart.transform.localPosition = Vector3.zero;
        movingKeyPart.transform.position += movingKeyPart.transform.position - movingKeyPart.GetPlugTransform().transform.position;
    }

    public void UnsnapKeyFromSocket()
    {
        Interactable1stPuzzleObject keyPointedAt = GetKeyPointedAt(out _);

        if (keyPointedAt == null || keyPointedAt.transform.TryGetComponent<Interactable1stPuzzleMainKey>(out _)) //disregard main key
        {
            InteractionPanelIndividual.Instance.RaiseInteractionPanelDeactivated(this);
            return;
        }

        if (hoveredKey != keyPointedAt) //check unsna
        {
            hoveredKey = keyPointedAt;

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
            SocketData snappedSocket = movingKeyPart.GetHostSocket();
            movingKeyPart.Unsnap();
            InteractionManager1stPuzzle.Instance.UnregisterCombinedKey(movingKeyPart);
            snappedSocket.isOccupied = false;
            snappedSocket.snappedKey = null;

            OnKeyUnsnappedFromSocket?.Invoke(this, new(snappedSocket, movingKeyPart));

            movingKeyPart.SetParent(InteractionManager1stPuzzle.Instance.GetKeysInitialParent());
            movingKeyPart.transform.localRotation = Quaternion.identity;
            isKeyFollowingMouse = true;
        }
    }

    private void SnapHandler_EditViewDeactivated(object sender, EventArgs e)
    {
        ResetEditViewRelatedElements();
    }

    private void ResetEditViewRelatedElements()
    {
        if (movingKeyPart != null)
        {
            movingKeyPart.gameObject.Hide();
            movingKeyPart = null;
        }

        hoveredKey = null;
        isKeyFollowingMouse = false;
    }

    private void SnapHandler_KeyDeselected(object sender, EventArgs e)
    {
        movingKeyPart.SetParent(InteractionManager1stPuzzle.Instance.GetKeysInitialParent());
        movingKeyPart.gameObject.Hide();
        movingKeyPart = null;
        isKeyFollowingMouse = false;
    }

    private void SnapHandler_InventorySlotClicked(object sender, UI_Puzzle1stObject inventorySlot)
    {
        if (movingKeyPart != null && movingKeyPart != inventorySlot.GetKeyPart()) //stop displaying the previously chosen key part
        {
            movingKeyPart.gameObject.Hide();
        }

        movingKeyPart = inventorySlot.GetKeyPart();
        movingKeyPart.gameObject.Show();
        isKeyFollowingMouse = true;
    }
}

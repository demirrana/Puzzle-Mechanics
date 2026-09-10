using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UI_Puzzle1Manager : MonoBehaviour
{
    #region Singleton and Events
    public static UI_Puzzle1Manager Instance { get; private set; }

    public class ChangeInHoveredObjectEventArgs : EventArgs
    {
        public UI_Puzzle1stObject PreviousInventorySlot { get; }
        public UI_Puzzle1stObject CurrentInventorySlot { get; }

        public ChangeInHoveredObjectEventArgs(UI_Puzzle1stObject previousInventorySlot, UI_Puzzle1stObject currentInventorySlot)
        {
            PreviousInventorySlot = previousInventorySlot;
            CurrentInventorySlot = currentInventorySlot;
        }
    }

    public event EventHandler OnInventoryPanelActivated;
    public event EventHandler OnInventoryPanelDeactivated;
    public event EventHandler<ChangeInHoveredObjectEventArgs> OnHoveredInventorySlotChanged;
    public event EventHandler OnNoHoveredInventorySlot;
    public event EventHandler<UI_Puzzle1stObject> OnInventorySlotClicked;
    #endregion

    #region Private Fields
    private UI_Puzzle1stObject currentlyHoveredSlot;
    private UI_Puzzle1stObject currentlyClickedSlot;
    #endregion

    #region Lifecycle Methods
    private void Awake()
    {
        SetInstance();
    }

    private void Start()
    {
        InteractionManager1stPuzzle.Instance.OnInteractableApproached += UI_Puzzle1Manager_InteractableApproached;
        InteractionManager1stPuzzle.Instance.OnNoInteractableNear += UI_Puzzle1Manager_NoInteractableNear;
        InteractionManager1stPuzzle.Instance.OnAnyKeyPartCollected += UI_Puzzle1Manager_KeyPartCollected;
        SnapHandler.Instance.OnKeySnappedToSocket += UI_Puzzle1Manager_KeySnappedToSocket;
        SnapHandler.Instance.OnSnappedSocketChanged += UI_Puzzle1Manager_SnappedSocketChanged;
        SnapHandler.Instance.OnKeyUnsnappedFromSocket += UI_Puzzle1Manager_KeyUnsnappedFromSocket;
        InteractionManager1stPuzzle.Instance.OnKeyDeselected += UI_Puzzle1Manager_KeyDeselected;
        InteractionManager1stPuzzle.Instance.OnEditViewActivated += UI_Puzzle1Manager_EditViewActivated;
        InteractionManager1stPuzzle.Instance.OnEditViewDeactivated += UI_Puzzle1Manager_EditViewDeactivated;
        InteractionManager1stPuzzle.Instance.OnDoorNear += UI_Puzzle1Manager_DoorNear;
    }

    private void OnDestroy()
    {
        InteractionManager1stPuzzle.Instance.OnInteractableApproached -= UI_Puzzle1Manager_InteractableApproached;
        InteractionManager1stPuzzle.Instance.OnNoInteractableNear -= UI_Puzzle1Manager_NoInteractableNear;
        InteractionManager1stPuzzle.Instance.OnAnyKeyPartCollected -= UI_Puzzle1Manager_KeyPartCollected;
        SnapHandler.Instance.OnKeySnappedToSocket -= UI_Puzzle1Manager_KeySnappedToSocket;
        SnapHandler.Instance.OnSnappedSocketChanged -= UI_Puzzle1Manager_SnappedSocketChanged;
        SnapHandler.Instance.OnKeyUnsnappedFromSocket -= UI_Puzzle1Manager_KeyUnsnappedFromSocket;
        InteractionManager1stPuzzle.Instance.OnKeyDeselected -= UI_Puzzle1Manager_KeyDeselected;
        InteractionManager1stPuzzle.Instance.OnEditViewActivated -= UI_Puzzle1Manager_EditViewActivated;
        InteractionManager1stPuzzle.Instance.OnEditViewDeactivated -= UI_Puzzle1Manager_EditViewDeactivated;
        InteractionManager1stPuzzle.Instance.OnDoorNear -= UI_Puzzle1Manager_DoorNear;
    }
    #endregion

    #region Slot Interaction Methods
    public bool IsMouseOverInventoryPanel()
    {
        PointerEventData eventData = new(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponentInParent<InventoryPanel>() != null)
                return true;
        }

        return false;
    }

    public void HandleSlotInteraction()
    {
        UI_Puzzle1stObject hoveredSlot = GetHoveredSlot();

        if (hoveredSlot == null)
        {
            OnNoHoveredInventorySlot?.Invoke(this, EventArgs.Empty);
            currentlyHoveredSlot = null;
            PreviewPanel.Instance.Hide();
            return;
        }

        DetectHoveredSlotChange(hoveredSlot);

        currentlyHoveredSlot = hoveredSlot;

        DetectClickOnInventorySlot();
    }

    private UI_Puzzle1stObject GetHoveredSlot()
    {
        PointerEventData eventData = new(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        UI_Puzzle1stObject foundSlot = null;
        foreach (var result in results)
        {
            foundSlot = result.gameObject.GetComponentInParent<UI_Puzzle1stObject>();

            if (foundSlot != null) //first found slot is saved
                break;
        }

        return foundSlot;
    }

    private void DetectHoveredSlotChange(UI_Puzzle1stObject hoveredSlot)
    {
        if (currentlyHoveredSlot != hoveredSlot)
        {
            ChangeInHoveredObjectEventArgs e = new(currentlyHoveredSlot, hoveredSlot);
            OnHoveredInventorySlotChanged?.Invoke(this, e);
            RectTransform previewPanelRectTransform = PreviewPanel.Instance.GetComponent<RectTransform>();
            RectTransform slotRectTransform = hoveredSlot.GetComponent<RectTransform>();
            Vector3 offset = new(220, 0, 0);
            previewPanelRectTransform.position = slotRectTransform.position + offset;
            PreviewPanel.Instance.Show();
        }
    }

    private void DetectClickOnInventorySlot()
    {
        if (currentlyHoveredSlot != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentlyClickedSlot != null) //show slot of previously clicked obj on inventory
                    currentlyClickedSlot.Show();

                OnInventorySlotClicked?.Invoke(this, currentlyHoveredSlot);
                currentlyClickedSlot = currentlyHoveredSlot;
                currentlyClickedSlot.Hide();
            }
        }
    }
    #endregion

    #region Event Handlers
    private void UI_Puzzle1Manager_InteractableApproached(object sender, Interactable<IInteractableBehaviour1stPuzzle> keyPart)
    {
        InteractableBehaviourCollectKeyPart collectKeyPart = new();
        InteractionPanelMovable.Instance.RaiseInteractionPanelActivated(sender, keyPart.transform, collectKeyPart.InteractionKeyCode);
    }

    private void UI_Puzzle1Manager_NoInteractableNear(object sender, EventArgs e)
    {
        InteractionPanelMovable.Instance.RaiseInteractionPanelDeactivated(sender);
    }

    private void UI_Puzzle1Manager_KeyPartCollected(object sender, Interactable1stPuzzleObject keyPart)
    {
        keyPart.gameObject.Hide();
        InventoryPanel.Instance.CreateNewSlot(keyPart);
    }

    private void UI_Puzzle1Manager_KeySnappedToSocket(object sender, SnapHandler.SnapToSocketEventArgs e)
    {
        currentlyClickedSlot.Hide();
        currentlyClickedSlot = null;
        InteractionPanelMovable.Instance.RaiseInteractionPanelDeactivated(sender);
    }

    private void UI_Puzzle1Manager_SnappedSocketChanged(object sender, SnapHandler.SnapToSocketEventArgs e)
    {
        SocketData socket = e.Socket;
        Interactable1stPuzzleObject snappedKey = e.SnappedKey;

        if (socket == null) //stop displaying key on previous socket
        {
            InteractionPanelMovable.Instance.RaiseInteractionPanelDeactivated(sender);
            return;
        }

        //display snap key on new socket
        InteractableBehaviourSnapToKeySocket snapToKey = new();
        Transform targetTransform = socket.socketTransform;
        KeyCode targetKey = snapToKey.InteractionKeyCode;
        InteractionPanelMovable.Instance.RaiseInteractionPanelActivated(this, targetTransform, targetKey);
    }

    private void UI_Puzzle1Manager_KeyUnsnappedFromSocket(object sender, SnapHandler.SnapToSocketEventArgs e)
    {
        InteractionPanelMovable.Instance.RaiseInteractionPanelDeactivated(sender);

        //find hidden slot of the unsnapped key and update currently clicked slot
        Interactable1stPuzzleObject unsnappedKey = e.SnappedKey;
        foreach (Transform slotTransform in InventoryPanel.Instance.GetContentTransform())
        {
            UI_Puzzle1stObject slot = slotTransform.GetComponent<UI_Puzzle1stObject>();
            
            if (slot.GetKeyPart() == unsnappedKey)
            {
                currentlyClickedSlot = slot;
            }
        }
    }

    private void UI_Puzzle1Manager_KeyDeselected(object sender, EventArgs e)
    {
        currentlyClickedSlot.Show();
        currentlyClickedSlot = null;
        InteractionPanelMovable.Instance.RaiseInteractionPanelDeactivated(sender);
    }

    private void UI_Puzzle1Manager_EditViewActivated(object sender, EventArgs e)
    {
        PlayerUIManager.Instance.HideExceptHand();
        MouseManager.Instance.ChangeMouseVisibility();

        InteractionPanelMovable.Instance.RaiseInteractionPanelDeactivated(sender);
        OnInventoryPanelActivated?.Invoke(sender, e);
        //maybe hide other objects around too
    }

    private void UI_Puzzle1Manager_EditViewDeactivated(object sender, EventArgs e)
    {
        PlayerUIManager.Instance.Show();
        MouseManager.Instance.ChangeMouseVisibility();

        OnInventoryPanelDeactivated?.Invoke(sender, e);
    }

    private void UI_Puzzle1Manager_DoorNear(object sender, Interactable1stPuzzleDoor door)
    {
        InteractionPanelMovable.Instance.RaiseInteractionPanelActivated(sender, door.transform, door.GetInteractionKey());
    }
    #endregion

    #region Helper Methods
    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    #endregion
}
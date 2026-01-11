using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UI_Puzzle1Manager : MonoBehaviour
{
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

    private UI_Puzzle1stObject currentlyHoveredSlot;
    private UI_Puzzle1stObject currentlyClickedSlot;

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
        InteractionManager1stPuzzle.Instance.OnInteractableApproached += UI_Puzzle1Manager_InteractableApproached;
        InteractionManager1stPuzzle.Instance.OnNoInteractableNear += UI_Puzzle1Manager_NoInteractableNear;
        InteractionManager1stPuzzle.Instance.OnAnyKeyPartCollected += UI_Puzzle1Manager_KeyPartCollected;
        InteractionManager1stPuzzle.Instance.OnKeySnappedToSocket += UI_Puzzle1Manager_KeySnappedToSocket;
        InteractionManager1stPuzzle.Instance.OnSnappedSocketChanged += UI_Puzzle1Manager_SnappedSocketChanged;
        InteractionManager1stPuzzle.Instance.OnKeyDeselected += UI_Puzzle1Manager_KeyDeselected;
        InteractionManager1stPuzzle.Instance.OnEditViewActivated += UI_Puzzle1Manager_EditViewActivated;
        InteractionManager1stPuzzle.Instance.OnEditViewDeactivated += UI_Puzzle1Manager_EditViewDeactivated;
        InteractionManager1stPuzzle.Instance.OnDoorNear += UI_Puzzle1Manager_DoorNear;
    }

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

    private void UI_Puzzle1Manager_InteractableApproached(object sender, Interactable<IInteractableBehaviour1stPuzzle> keyPart)
    {
        InteractableBehaviourCollectKeyPart collectKeyPart = new();
        InteractionPanelIndividual.Instance.RaiseInteractionPanelActivated(sender, keyPart.transform, collectKeyPart.InteractionKeyCode);
    }

    private void UI_Puzzle1Manager_NoInteractableNear(object sender, EventArgs e)
    {
        InteractionPanelIndividual.Instance.RaiseInteractionPanelDeactivated(sender);
    }

    private void UI_Puzzle1Manager_KeyPartCollected(object sender, Interactable1stPuzzleObject keyPart)
    {
        keyPart.gameObject.Hide();
        InventoryPanel.Instance.CreateNewSlot(keyPart);
    }

    private void UI_Puzzle1Manager_KeySnappedToSocket(object sender, InteractionManager1stPuzzle.SnapToSocketEventArgs e)
    {
        currentlyClickedSlot.Hide();
        currentlyClickedSlot = null;
        InteractionPanelIndividual.Instance.RaiseInteractionPanelDeactivated(sender);
    }

    private void UI_Puzzle1Manager_SnappedSocketChanged(object sender, InteractionManager1stPuzzle.SnapToSocketEventArgs e)
    {
        SocketData socket = e.Socket;
        Interactable1stPuzzleObject snappedKey = e.SnappedKey;

        if (socket == null) //stop displaying key on previous socket
        {
            InteractionPanelIndividual.Instance.RaiseInteractionPanelDeactivated(sender);
            return;
        }

        //display snap key on new socket
        InteractableBehaviourSnapToKeySocket snapToKey = new();
        Transform targetTransform = socket.socketTransform;
        KeyCode targetKey = snapToKey.InteractionKeyCode;
        InteractionPanelIndividual.Instance.RaiseInteractionPanelActivated(this, targetTransform, targetKey);
    }

    private void UI_Puzzle1Manager_KeyDeselected(object sender, EventArgs e)
    {
        currentlyClickedSlot.Show();
        currentlyClickedSlot = null;
        InteractionPanelIndividual.Instance.RaiseInteractionPanelDeactivated(sender);
    }

    private void UI_Puzzle1Manager_EditViewActivated(object sender, EventArgs e)
    {
        PlayerUIManager.Instance.HideExceptHand();
        MouseManager.Instance.ChangeMouseVisibility();

        InteractionPanelIndividual.Instance.RaiseInteractionPanelDeactivated(sender);
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
        InteractionPanelIndividual.Instance.RaiseInteractionPanelActivated(sender, door.transform, door.GetInteractionKey());
    }
}
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

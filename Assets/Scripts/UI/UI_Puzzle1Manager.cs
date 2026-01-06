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

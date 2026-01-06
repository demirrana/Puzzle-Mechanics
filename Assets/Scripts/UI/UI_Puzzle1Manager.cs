using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UI_Puzzle1Manager : MonoBehaviour
{
    public static UI_Puzzle1Manager Instance { get; private set; }

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

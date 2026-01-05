using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private Button inventoryButton;
    [SerializeField] private ScrollRect slotContainer;
    [SerializeField] private Transform contentTransform;

    [SerializeField] private UI_Puzzle1stObject inventorySlotPrefab;

    public static InventoryPanel Instance { get; private set; }

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
    
    private void HideInventoryContent()
    {
        slotContainer.gameObject.SetActive(false);
    }

    private void Start()
    {
        inventoryButton.onClick.AddListener(ToggleSlotContainerVisibility);
    }

    private void ToggleSlotContainerVisibility()
    {
        if (IsInventoryActive())
        {
            HideInventoryContent();
        }
        else
        {
            ShowInventoryContent();
        }
    }

    private bool IsInventoryActive()
    {
        return slotContainer.gameObject.activeSelf;
    }


    private void ShowInventoryContent()
    {
        slotContainer.gameObject.SetActive(true);
    }


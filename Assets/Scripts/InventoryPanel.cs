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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleSlotContainerVisibility();
            MouseManager.Instance.ChangeMouseVisibility();
        }
    }

    public void HideInventoryButton()
    {
        inventoryButton.gameObject.SetActive(false);
    }

    public void ShowInventoryButton()
    {
        inventoryButton.gameObject.SetActive(true);
    }


    public void CreateNewSlot(Interactable1stPuzzleObject keyPart)
    {
        SOCollectibleKeyPart keyPartData = keyPart.GetKeyPartData();
        UI_Puzzle1stObject keyPartUI = Instantiate(inventorySlotPrefab, contentTransform);
        keyPartUI.Setup(keyPartData, keyPart);
    }

    private void ShowInventoryContent()
    {
        slotContainer.gameObject.SetActive(true);
    }
}
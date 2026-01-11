using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private Button inventoryButton;
    [SerializeField] private ScrollRect slotContainer;
    [SerializeField] private Transform contentTransform;

    [SerializeField] private UI_Puzzle1stObject inventorySlotPrefab;

    public static InventoryPanel Instance { get; private set; }

    public Transform GetContentTransform()
    {
        return contentTransform;
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
    
    private void HideInventoryContent()
    {
        slotContainer.gameObject.SetActive(false);
    }

    private void Start()
    {
        UI_Puzzle1Manager.Instance.OnInventoryPanelActivated += InventoryPanel_InventoryPanelActivated;
        UI_Puzzle1Manager.Instance.OnInventoryPanelDeactivated += InventoryPanel_InventoryPanelDeactivated;

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

    private void InventoryPanel_InventoryPanelActivated(object sender, EventArgs e)
    {
        ShowInventoryButton();
    }

    private void InventoryPanel_InventoryPanelDeactivated(object sender, EventArgs e)
    {
        HideInventoryContent();
        HideInventoryButton();
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
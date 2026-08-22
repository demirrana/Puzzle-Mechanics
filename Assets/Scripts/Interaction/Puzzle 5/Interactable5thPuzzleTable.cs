using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable5thPuzzleTable : MonoBehaviour
{
    #region Singleton & Events
    public static Interactable5thPuzzleTable Instance { get; private set; }

    public event EventHandler OnTableViewActivated;
    public event EventHandler OnTableViewDeactivated;
    #endregion

    #region Fields
    [Header("Input Settings")]
    private readonly KeyCode OpenTableViewKey = KeyCode.E;
    private readonly KeyCode CloseTableViewKey = KeyCode.F;

    [Header("Slot Management")]
    private List<Interactable5thPuzzleTableSlot> emptySlots = new(); //table slots without an interactable object on them
    private List<Interactable5thPuzzleTableSlot> occupiedSlots = new(); //table slots with an interactable object on them
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        SetInstance();
    }

    private void Start()
    {
        OnTableViewActivated += ToggleTableView;
        OnTableViewDeactivated += ToggleTableView;
        foreach (Interactable5thPuzzleTableSlot slot in GetComponentsInChildren<Interactable5thPuzzleTableSlot>())
        {
            emptySlots.Add(slot);
        }
    }
    #endregion

    #region Public API - Table View & Events
    public void RaiseTableViewActivated()
    {
        Debug.Log("Table view is activated.");
        OnTableViewActivated?.Invoke(this, null);
    }

    public void RaiseTableViewDeactivated()
    {
        Debug.Log("Table view is deactivated.");
        OnTableViewDeactivated?.Invoke(this, null);
    }

    public void DetectOpenTableView() //for when table opening can't be controlled through object behaviours since no object is in hand
    {
        if (IsTableViewOpenKeyPressed())
            RaiseTableViewActivated();
    }

    public void DetectCloseTableView() //for when table closing can't be controlled through object behaviours since no object is in hand
    {
        if (IsTableViewCloseKeyPressed())
            RaiseTableViewDeactivated();
    }

    public KeyCode GetKeyForOpenTableView() => OpenTableViewKey;
    public KeyCode GetKeyForCloseTableView() => CloseTableViewKey;
    #endregion

    #region Public API - Slot Management
    public void TransferSlotToEmptySlots(Interactable5thPuzzleTableSlot slot) //update slot state after object is removed from the slot
    {
        //Debug.Log("Slot is empty now: " + slot.name);
        emptySlots.Add(slot);
        occupiedSlots.Remove(slot);
        // Debug.Log("Empty Slots:");
        // foreach (Interactable5thPuzzleTableSlot s in emptySlots)
        // {
        //     Debug.Log(s.name);
        // }
    }

    public void TransferSlotToFullSlots(Interactable5thPuzzleTableSlot slot) //update slot state after object is placed on the slot
    {
        //Debug.Log("Slot is full now: " + slot.name);
        occupiedSlots.Add(slot);
        emptySlots.Remove(slot);
        // Debug.Log("Full Slots:");
        // foreach (Interactable5thPuzzleTableSlot s in occupiedSlots)
        // {
        //     Debug.Log(s.name);
        // }
    }

    public bool HasEmptySlots() => emptySlots.Count > 0;
    public bool HasFullSlots() => occupiedSlots.Count > 0;

    public Interactable5thPuzzleTableSlot GetPointedEmptySlot() => GetPointedSlot(emptySlots);
    public Interactable5thPuzzleTableSlot GetPointedFullSlot() => GetPointedSlot(occupiedSlots);

    //for debugging (to be deleted)
    public void LogEmptyAndFullSlots()
    {
        Debug.Log("Empty Slots:");
        foreach (Interactable5thPuzzleTableSlot s in emptySlots)
        {
            Debug.Log(s.name);
        }
        Debug.Log("Full Slots:");
        foreach (Interactable5thPuzzleTableSlot s in occupiedSlots)
        {
            Debug.Log(s.name);
        }
    }
    #endregion

    #region Private Helper Methods
    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private bool IsTableViewOpenKeyPressed() => Input.GetKeyDown(OpenTableViewKey);

    private bool IsTableViewCloseKeyPressed() => Input.GetKeyDown(CloseTableViewKey);

    //Gets each slot's projected position onto the active camera and gives the closest one to the mouse.
    private Interactable5thPuzzleTableSlot GetPointedSlot(List<Interactable5thPuzzleTableSlot> slotList)
    {
        Vector3 mousePosition = Input.mousePosition;

        return slotList
        .Select(slot => new
        {
            Slot = slot,
            ScreenPos = Camera.main.WorldToScreenPoint(slot.transform.position)
        })
        .Where(x => x.ScreenPos.z >= 0) // Ignore behind-camera objects
        .OrderBy(x => Vector2.Distance(
            new Vector2(x.ScreenPos.x, x.ScreenPos.y),
            new Vector2(mousePosition.x, mousePosition.y)))
        .FirstOrDefault()?.Slot;
    }
    #endregion

    #region Event Handlers
    private void ToggleTableView(object sender, EventArgs e)
    {
        CameraManager.Instance.SwitchToNextCamera();
        InteractionManager5thPuzzle.Instance.ChangeGameState();
        MouseManager.Instance.ChangeMouseVisibility();
        if (PlayerMovementManager.Instance.enabled) PlayerScriptsManager.Instance.DisableMovementScript();
        else PlayerScriptsManager.Instance.EnableMovementScript();
    }
    #endregion
}
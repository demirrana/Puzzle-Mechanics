using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable5thPuzzleTable : MonoBehaviour
{
    public static Interactable5thPuzzleTable Instance { get; private set; }

    public event EventHandler OnTableViewActivated;
    public event EventHandler OnTableViewDeactivated;

    private List<Interactable5thPuzzleTableSlot> emptySlots = new(); //these are gonna be changed to Interactable5thPuzzleTableSlot
    private List<Interactable5thPuzzleTableSlot> fullSlots = new();

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

    public void RaiseTableViewActivated()
    {
        OnTableViewActivated?.Invoke(this, null);
    }

    public void RaiseTableViewDeactivated()
    {
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

    public void TransferSlotToEmptySlots(Interactable5thPuzzleTableSlot slot)
    {
        Debug.Log("Slot is empty now: " + slot.name);
        emptySlots.Add(slot);
        fullSlots.Remove(slot);
        Debug.Log("Empty Slots:");
        foreach (Interactable5thPuzzleTableSlot s in emptySlots)
        {
            Debug.Log(s.name);
        }
    }

    public void TransferSlotToFullSlots(Interactable5thPuzzleTableSlot slot)
    {
        Debug.Log("Slot is full now: " + slot.name);
        fullSlots.Add(slot);
        emptySlots.Remove(slot);
        Debug.Log("Full Slots:");
        foreach (Interactable5thPuzzleTableSlot s in fullSlots)
        {
            Debug.Log(s.name);
        }
    }

    //for debugging (to be deleted)
    public void LogEmptyAndFullSlots()
    {
        Debug.Log("Empty Slots:");
        foreach (Interactable5thPuzzleTableSlot s in emptySlots)
        {
            Debug.Log(s.name);
        }
        Debug.Log("Full Slots:");
        foreach (Interactable5thPuzzleTableSlot s in fullSlots)
        {
            Debug.Log(s.name);
        }
    }

    private bool IsTableViewOpenKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    private bool IsTableViewCloseKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.F);
    }

    public bool HasEmptySlots()
    {
        return emptySlots.Count > 0;
    }

    public bool HasFullSlots()
    {
        return fullSlots.Count > 0;
    }

    public Interactable5thPuzzleTableSlot GetPointedEmptySlot() //ray from mouse's position in the direction of camera 
    {
        return GetPointedSlot(emptySlots);
    }

    public Interactable5thPuzzleTableSlot GetPointedFullSlot() //ray from mouse's position in the direction of camera 
    {
        return GetPointedSlot(fullSlots);
    }

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

    private void ToggleTableView(object sender, EventArgs e)
    {
        CameraManager.Instance.SwitchToNextCamera();
        InteractionManager5thPuzzle.Instance.ChangeGameState();
        MouseManager.Instance.ChangeMouseVisibility();
        if (PlayerMovementManager.Instance.enabled) PlayerScriptsManager.Instance.DisableMovementScript();
        else PlayerScriptsManager.Instance.EnableMovementScript();
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
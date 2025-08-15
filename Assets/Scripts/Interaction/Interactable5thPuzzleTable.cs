using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        OnTableViewActivated += ToggleTableView;
        OnTableViewDeactivated += ToggleTableView;

    public void RaiseTableViewActivated()
    {
        OnTableViewActivated?.Invoke(this, null);
    }

    public void RaiseTableViewDeactivated()
    {
        OnTableViewDeactivated?.Invoke(this, null);
    }

    public bool HasEmptySlots()
    {
        return emptySlots.Count > 0;
    }

    public Interactable5thPuzzleTableSlot GetPointedEmptySlot() //ray from mouse's position in the direction of camera 
    {
        emptySlots = emptySlots.OrderBy(d => GetSlotDistanceToMouse(d)).ToList();
        return emptySlots[0];
    }

    public Interactable5thPuzzleTableSlot GetPointedFullSlot() //ray from mouse's position in the direction of camera 
    {
        fullSlots = fullSlots.OrderBy(d => GetSlotDistanceToMouse(d)).ToList();
        return fullSlots[0];
    }

    private void ToggleTableView(object sender, EventArgs e)
    {
        CameraManager.Instance.SwitchToNextCamera();
        InteractionManager5thPuzzle.Instance.ChangeGameState();
        InteractionManager5thPuzzle.Instance.ChangeMouseVisibility();
    }

    private float GetSlotDistanceToMouse(Interactable5thPuzzleTableSlot slot)
    {
        return Vector3.Distance(slot.transform.position, Input.mousePosition);
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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable5thPuzzleTable : Interactable5thPuzzle
{
    public static Interactable5thPuzzleTable Instance { get; private set; }

    private List<Interactable5thPuzzleTableSlot> emptySlots = new(); //these are gonna be changed to Interactable5thPuzzleTableSlot
    private List<Interactable5thPuzzleTableSlot> fullSlots = new();

    private void Awake()
    {
        SetInstance();
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

    protected override void GetInteracted_Interactable(object sender, IInteractableBehaviour5thPuzzle interactionBehaviour)
    private void ToggleTableView(object sender, EventArgs e)
    {
        //Debug.Log($"[Child Handler] invoked on {name}, target type: {GetType().Name} (InstanceID {GetInstanceID()})");
        Debug.Log("GetInteracted_Interactable of Interactable5thPuzzleTable is called.");
        interactionBehaviour.Interact<IInteractableBehaviour5thPuzzle>(this);
        Debug.Log("Behaviour keycode: " + interactionBehaviour.InteractionKeyCode.ToString());
        CameraManager.Instance.SwitchToNextCamera();
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
using UnityEngine;

// <summary>
// This class represents a slot on the 5th puzzle table that can hold an interactable object
public class Interactable5thPuzzleTableSlot : MonoBehaviour
{
    [SerializeField] private Transform slotTransform;

    private Interactable5thPuzzleObject interactableOnSlot = null;

    public Interactable5thPuzzleObject GetInteractableOnSlot()
    {
        return interactableOnSlot;
    }

    public void SetInteractableInSlot(Interactable5thPuzzleObject interactableObject)
    {
        interactableOnSlot = interactableObject;
    }
}
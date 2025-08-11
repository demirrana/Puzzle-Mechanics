using UnityEngine;

public class Interactable5thPuzzleTableSlot : Interactable
{
    [SerializeField] private Transform slotTransform;

    private Interactable5thPuzzle interactableOnSlot;

    private void Awake()
    {
        //behavioursList.Add(new GetFull());
    }

    public Interactable5thPuzzle GetInteractableOnSlot()
    {
        return interactableOnSlot;
    }
}
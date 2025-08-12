using UnityEngine;

public class Interactable5thPuzzleTableSlot : Interactable5thPuzzle
{
    [SerializeField] private Transform slotTransform;

    private Interactable5thPuzzleObject interactableOnSlot;

    private void Awake()
    {
        //behavioursList.Add(new GetFull());
    }

    public Interactable5thPuzzleObject GetInteractableOnSlot()
    {
        return interactableOnSlot;
    }
}
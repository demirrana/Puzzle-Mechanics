using UnityEngine;

public class Interactable5thPuzzleTableSlot : MonoBehaviour
{
    [SerializeField] private Transform slotTransform;

    private Interactable5thPuzzleObject interactableOnSlot;

    private void Awake()
    {
    }

    public Interactable5thPuzzleObject GetInteractableOnSlot()
    {
        return interactableOnSlot;
    }
}
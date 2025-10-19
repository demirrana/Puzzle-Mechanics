using UnityEngine;

public class Interactable0thPuzzleObject : Interactable0thPuzzle
{
    public SOBookData bookData;
    [SerializeField] private Vector3 bookPlaceOnShelf;
    public Vector3 GetBookPlaceOnShelf()
    {
        return bookPlaceOnShelf;
    }

}

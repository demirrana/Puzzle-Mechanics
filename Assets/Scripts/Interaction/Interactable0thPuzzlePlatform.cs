using System.Collections.Generic;
using UnityEngine;

public class Interactable0thPuzzlePlatform : Interactable0thPuzzle
{
    public static Interactable0thPuzzlePlatform Instance { get; private set; }

    [SerializeField] private Transform topCenterPointTransform; //should be set to that point + (book thickness / 2)

    private List<Interactable0thPuzzleObject> booksOnPlatform;
    private float yValueToPutNextBook = 0f;
    private static readonly float BookThickness = 0.03f;

    private readonly int BookCapacity = 8;

    private void Awake()
    {
        SetInstance();
    }

    public Vector3 GetNextBookPos()
    {
        if (booksOnPlatform.Count == 0)
        {
            return topCenterPointTransform.position;
        }
        else
        {
            Vector3 newPosition = topCenterPointTransform.position;
            newPosition.y = yValueToPutNextBook;
            return newPosition;
        }
    }

    public bool HasPlace()
    {
        return booksOnPlatform.Count < BookCapacity;
    }

    public bool HasBooks()
    {
        return booksOnPlatform.Count > 0;
    }

    public bool HasTheBook(Interactable0thPuzzleObject book)
    {
        return booksOnPlatform.Contains(book);
    }

    public void AddBookAndUpdate(Interactable0thPuzzleObject newBook)
    {
        AddBook(newBook);
        UpdateAddedBookPosition();
        UpdateAfterAdd_yValueForNextBook();
    }

    private void AddBook(Interactable0thPuzzleObject newBook)
    {
        booksOnPlatform.Add(newBook);
    }

    private void RemoveBook(Interactable0thPuzzleObject book)
    {
        booksOnPlatform.Remove(book);
    }

    private void UpdateAddedBookPosition()
    {
        Interactable0thPuzzleObject addedBook = booksOnPlatform[-1];
        Vector3 addedBookPosition = addedBook.GetPosition();
        addedBookPosition.y = yValueToPutNextBook;
        addedBook.SetPosition(addedBookPosition);
    }

    private void ShiftBooksDownAfterRemoval(int removedBookOrderOnPlatform)
    {
        int currentBookOrder = removedBookOrderOnPlatform;
        List<Interactable0thPuzzleObject> booksFloatingAfterRemoval = booksOnPlatform.GetRange(removedBookOrderOnPlatform - 1, booksOnPlatform.Count);
        foreach (Interactable0thPuzzleObject book in booksFloatingAfterRemoval)
        {
            Vector3 newBookPosition = book.GetPosition();
            float yBookValue = currentBookOrder * BookThickness;
            newBookPosition.y = yBookValue;
            book.SetPosition(newBookPosition);
        }
    }

    private void UpdateAfterAdd_yValueForNextBook()
    {
        yValueToPutNextBook += BookThickness;
    }
    
    private void UpdateAfterRemove_yValueForNextBook()
    {
        yValueToPutNextBook -= BookThickness;
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

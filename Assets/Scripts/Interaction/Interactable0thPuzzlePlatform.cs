using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable0thPuzzlePlatform : MonoBehaviour
{
    public static Interactable0thPuzzlePlatform Instance { get; private set; }

    [SerializeField] private Transform topCenterPointTransform; //should be set to that point + (book thickness / 2)

    private List<Interactable<IInteractableBehaviour0thPuzzle>> booksOnPlatform = new();
    private float yValueToPutNextBook;
    private static readonly float BookThickness = 0.03f;

    private static readonly KeyCode EnterPlatformViewKey = KeyCode.E;
    private static readonly KeyCode ExitPlatformViewKey = KeyCode.Q; //TO BE CHANGED TO ESCAPE

    private readonly int BookCapacity = 8;

    private void Awake()
    {
        SetInstance();
    }

    private void Start()
    {
        InitializeValues();
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

    public Interactable0thPuzzleObject GetPointedBook()
    {
        if (!HasBooks())
        {
            return null;
        }
        else
        {
            Interactable<IInteractableBehaviour0thPuzzle> pointedBook = null;
            Vector3 mousePositionInWorld = MouseManager.Instance.GetMousePositionInWorld();
            pointedBook = booksOnPlatform
                .OrderBy(pointedBook => Vector3.Distance(pointedBook.transform.position, mousePositionInWorld))
                .FirstOrDefault();
            return pointedBook as Interactable0thPuzzleObject;
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
        //UpdateAddedBookPosition(); this is handled by behaviour 
        UpdateAfterAdd_yValueForNextBook();
    }

    public void RemoveBookAndUpdate(Interactable0thPuzzleObject book)
    {
        int removedBookOrder = booksOnPlatform.IndexOf(book) + 1;
        RemoveBook(book);
        ShiftBooksDownAfterRemoval(removedBookOrder);
        UpdateAfterRemove_yValueForNextBook();
    }

    private void AddBook(Interactable0thPuzzleObject newBook)
    {
        booksOnPlatform.Add(newBook);
    }

    private void RemoveBook(Interactable0thPuzzleObject book)
    {
        booksOnPlatform.Remove(book);
    }

    public bool IsEnterPlatformViewKeyPressed()
    {
        return Input.GetKeyDown(EnterPlatformViewKey);
    }

    public bool IsExitPlatformViewKeyPressed()
    {
        return Input.GetKeyDown(ExitPlatformViewKey);
    }

    private void UpdateAddedBookPosition()
    {
        Interactable0thPuzzleObject addedBook = booksOnPlatform[^1] as Interactable0thPuzzleObject;
        if (addedBook != null)
        {  
            Vector3 addedBookPosition = topCenterPointTransform.position;
            addedBookPosition.y = yValueToPutNextBook;
            addedBook.SetPosition(addedBookPosition);
        }
    }

    private void ShiftBooksDownAfterRemoval(int removedBookOrderOnPlatform)
    {
        int currentBookOrder = removedBookOrderOnPlatform;
        List<Interactable<IInteractableBehaviour0thPuzzle>> booksToShiftDown;
        booksToShiftDown = booksOnPlatform.GetRange(removedBookOrderOnPlatform - 1, booksOnPlatform.Count);

        foreach (Interactable0thPuzzleObject book in booksToShiftDown.OfType<Interactable<IInteractableBehaviour0thPuzzle>>())
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
    
    private void InitializeValues()
    {
        yValueToPutNextBook = topCenterPointTransform.position.y;
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

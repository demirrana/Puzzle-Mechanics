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

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Interactable0thPuzzlePlatform : Interactable0thPuzzle
{
    public static Interactable0thPuzzlePlatform Instance { get; private set; }

    private List<Interactable0thPuzzleObject> booksOnPlatform;

    private readonly int BookCapacity = 8;

    private void Awake()
    {
        SetInstance();
    }

    public bool HasPlace()
    {
        return booksOnPlatform.Count < BookCapacity;
    }

    public bool HasBooks()
    {
        return booksOnPlatform.Count > 0;
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

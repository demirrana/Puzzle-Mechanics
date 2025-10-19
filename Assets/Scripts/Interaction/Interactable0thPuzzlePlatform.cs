using System.Collections.Generic;
using UnityEngine;

public class Interactable0thPuzzlePlatform : Interactable0thPuzzle
{
    public static Interactable0thPuzzlePlatform Instance { get; private set; }
    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
}

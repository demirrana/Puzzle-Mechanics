using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager0thPuzzle : InteractionManager<IInteractableBehaviour0thPuzzle> //to be changed to 0thPuzzle
{
    public static InteractionManager0thPuzzle Instance { get; private set; }

    [SerializeField] private Transform puzzle0ObjectsHolder;

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
}

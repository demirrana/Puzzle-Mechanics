using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public static InteractionManager7thPuzzle Instance { get; private set; }

    public event EventHandler<List<Collider>> OnObjectCollidersApproached;
    public event EventHandler OnNoInteractableNear;
    public event EventHandler<Interactable7thPuzzleObject> OnInteractableApproached;
    private void Awake()
    {
        SetInstance();
    }
    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

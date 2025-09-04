using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public static InteractionManager7thPuzzle Instance { get; private set; }

    public event EventHandler<List<Collider>> OnObjectCollidersApproached;
    public event EventHandler OnNoInteractableNear;
    public event EventHandler<Interactable7thPuzzleObject> OnInteractableApproached;

    [SerializeField] private ParticleSystem choosingCircle1;
    [SerializeField] private ParticleSystem choosingCircle2;

    private InteractionPanelIndividual InteractionPanelIndividual;
    private Interactable7thPuzzleObject interactable1stChosen;
    private Interactable7thPuzzleObject interactable2ndChosen; //2nd interactable chosen to swap with the one in hand
    private Interactable7thPuzzleObject approachedInteractable;

    private void Awake()
    {
        SetInstance();
    }

    protected override void Start()
    {
        base.Start();
        InitializeInstances();
        OnObjectCollidersApproached += ObjectCollidersApproached_PlayerInteractionManager7thPuzzle;
        OnInteractableApproached += InteractableApproached_PlayerInteractionManager7thPuzzle;
        OnNoInteractableNear += NoInteractableNear_PlayerInteractionManager7thPuzzle;
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

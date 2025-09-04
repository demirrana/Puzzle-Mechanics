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


    public void ChooseInteractable(Interactable7thPuzzleObject interactable)
    {
        if (interactable1stChosen == null)
        {
            interactable1stChosen = interactable;
        }
        else
        {
            interactable2ndChosen = interactable;
        }
    }

    public void DeselectInteractable(Interactable7thPuzzleObject interactable)
    {
        if (interactable1stChosen == interactable)
        {
            interactable1stChosen = interactable2ndChosen;
        }

        interactable2ndChosen = null;
    }

    public void PlayChoosingVFX(Vector3 targetPosition)
    {
        if (choosingCircle1.isPlaying && choosingCircle2.isPlaying) //already playing
            return;

        Vector3 vfxPosition = new(targetPosition.x, 0.2f, targetPosition.z);

        if (choosingCircle1.isPlaying)
        {
            //Debug.Log("circle 1 is already playing");
            choosingCircle2.transform.position = vfxPosition;
            choosingCircle2.Play();
        }
        else
        {
            //Debug.Log("circle 2 is already playing");
            choosingCircle1.transform.position = vfxPosition;
            choosingCircle1.Play();
        }
    }

    public void StopChoosingVFX(ParticleSystem vfx)
    {
        //Debug.Log("StopChoosingVFX is called for " + vfx.ToString());
        if (vfx.isPlaying)
            vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

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

    //Makes sure interactable1stInHand is the chosen when there is only 1 chosen interactables since it represents the first chosen interactable
    private void Detect1stInteractableDeselected()
    {
        if (interactable1stChosen == null && interactable2ndChosen != null)
        {
            interactable1stChosen = interactable2ndChosen;
            interactable2ndChosen = null;
        }
    }

    protected void InteractableApproached_PlayerInteractionManager7thPuzzle(object sender, Interactable7thPuzzleObject interactable)
    {
        bool isApproachedChosen = interactable != null && (interactable == interactable1stChosen || interactable == interactable2ndChosen);
        bool areTwoInteractablesChosen = interactable1stChosen != null && interactable2ndChosen != null;
        if (areTwoInteractablesChosen && !isApproachedChosen) //when 2 are already chosen, approached is used for only deselecting
        {
            SetApproachedInteractable(null);
            return;
        }
        
        SetApproachedInteractable(interactable);
        //Debug.Log("InteractableApproached and vfx is displayed. The approached interactable is " + interactable.ToString());
        PlayChoosingVFX(interactable.transform.position);
        IInteractableBehaviour7thPuzzle interactionBehaviour = interactable.GetInteractionBehaviours()[0]; //there can be only 1 behaviour
        ActivateInteractionPanel(this, interactable.transform, interactionBehaviour.InteractionKeyCode);
    }

    private void ActivateInteractionPanel(object sender, Transform targetTransform, KeyCode interactionKey)
    {
        InteractionPanelIndividual.RaiseInteractionPanelActivated(sender, targetTransform, interactionKey);
    }

    private void DeactivateInteractionPanel()
    {
        InteractionPanelIndividual.RaiseInteractionPanelDeactivated(this);
    }

    private void SetApproachedInteractable(Interactable7thPuzzleObject interactable)
    {
        approachedInteractable = interactable;
    }

    private void InitializeInstances()
    {
        interactable1stChosen = interactableInHand as Interactable7thPuzzleObject;
        InteractionPanelIndividual = InteractionPanelIndividual.Instance;
        interactable2ndChosen = null;
        approachedInteractable = null;
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
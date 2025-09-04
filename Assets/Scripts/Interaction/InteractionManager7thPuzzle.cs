using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager7thPuzzle : InteractionManager<IInteractableBehaviour7thPuzzle>
{
    public static InteractionManager7thPuzzle Instance { get; private set; }

    public event EventHandler<List<Collider>> OnObjectCollidersApproached;
    public event EventHandler OnNoInteractableNear;
    public event EventHandler<Interactable7thPuzzleObject> OnInteractableApproached;
    public event EventHandler<KeyCode> OnBothInteractablesAreChosen;
    public event EventHandler OnAnyInteractableIsDeselected;

    [SerializeField] private ParticleSystem choosingCircle1;
    [SerializeField] private ParticleSystem choosingCircle2;

    private InteractionPanelIndividual InteractionPanelIndividual;
    private Interactable7thPuzzleObject interactable1stChosen;
    private Interactable7thPuzzleObject interactable2ndChosen; //2nd interactable chosen to swap with the one in hand
    private Interactable7thPuzzleObject approachedInteractable;
    private KeyCode swapKey;

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

    private void Update()
    {
        //if (approachedInteractable != null)
        //approachedInteractable.LogBehaviours();
        DetectInteractionConditionsMet();
        //LogChosenOnes();
    }

    private void LogChosenOnes()
    {
        if (interactable1stChosen == null)
            Debug.Log("1st is null.");
        else
            Debug.Log("1st is " + interactable1stChosen.ToString());
        if (interactable2ndChosen == null)
            Debug.Log("2nd is null.");
        else
            Debug.Log("2nd is " + interactable2ndChosen.ToString());
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

        if (interactable2ndChosen != null)
        {
            OnBothInteractablesAreChosen?.Invoke(this, swapKey);
        }
    }

    public void DeselectInteractable(Interactable7thPuzzleObject interactable)
    {
        if (interactable1stChosen == interactable)
        {
            interactable1stChosen = interactable2ndChosen;
        }

        interactable2ndChosen = null;

        OnAnyInteractableIsDeselected?.Invoke(this, EventArgs.Empty);
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

    protected override void DetectInteractionConditionsMet()
    {
        DetectAnyColliderApproached();

        if (interactable1stChosen == null) //no interactables are chosen
        {
            //Debug.Log("No chosen interactables.");
            DetectInteraction_NoInteractableChosen(approachedInteractable);
        }
        else if (interactable2ndChosen == null) //only one interactable is chosen
        {
            //Debug.Log("One chosen interactables.");
            DetectInteraction_OneInteractableChosen(approachedInteractable);
        }
        else //both interactables are already chosen
        {
            //Debug.Log("Two chosen interactables.");
            DetectInteraction_TwoInteractableChosen(approachedInteractable);
        }
    }

    private void DetectInteraction_NoInteractableChosen(Interactable<IInteractableBehaviour7thPuzzle> approachedInteractable)
    {
        DetectChoosingInteractable(approachedInteractable);
    }

    private void DetectInteraction_OneInteractableChosen(Interactable<IInteractableBehaviour7thPuzzle> approachedInteractable)
    {
        bool isApproachedChosen = approachedInteractable == interactable1stChosen || approachedInteractable == interactable2ndChosen;
        if (approachedInteractable != null && isApproachedChosen)
            DetectDeselectingInteractable(approachedInteractable);
        else
            DetectChoosingInteractable(approachedInteractable);
    }

    private void DetectInteraction_TwoInteractableChosen(Interactable<IInteractableBehaviour7thPuzzle> approachedInteractable)
    {
        DetectDeselectingInteractable(approachedInteractable);
        DetectSwapping();
    }

    private void DetectChoosingInteractable(Interactable<IInteractableBehaviour7thPuzzle> approachedInteractable)
    {
        if (approachedInteractable != null)
        {
            //Debug.Log("There is an approached one and behaviour is checked.");
            DetectBehaviourApplied(approachedInteractable, new InteractableBehaviourBeChosen());
        }
    }

    private void DetectDeselectingInteractable(Interactable<IInteractableBehaviour7thPuzzle> approachedInteractable)
    {
        if (approachedInteractable != null)
        {
            if (approachedInteractable == interactable1stChosen)
            {
                DetectBehaviourApplied(interactable1stChosen, new InteractableBehaviourBeDeselected());
            }
            else if (approachedInteractable == interactable2ndChosen)
            {
                DetectBehaviourApplied(interactable2ndChosen, new InteractableBehaviourBeDeselected());
            }
        }
    }

    private void DetectSwapping()
    {
        if (IsSwapKeyPressed())
        {
            StartCoroutine(ApplySwappingBetweenObjects());
        }
    }

    private bool IsSwapKeyPressed()
    {
        return Input.GetKeyDown(swapKey);
    }


    protected void DetectAnyColliderApproached()
    {
        List<Collider> hitColliders = GetCollidersApproached();

        switch (hitColliders.Count)
        {
            case 0:
                OnNoInteractableNear?.Invoke(this, null);
                break;
            default:
                OnObjectCollidersApproached?.Invoke(this, hitColliders);
                break;
        }
    }

    private void NoInteractableNear_PlayerInteractionManager7thPuzzle(object sender, EventArgs e)
    {
        DeactivateInteractionPanel();
        SetApproachedInteractable(null);
        ManageStoppingChoosingVFX_NoInteractableNear();
    }

    private void ManageStoppingChoosingVFX_NoInteractableNear()
    {
        bool isOnly1stInteractableChosen = interactable1stChosen != null && interactable2ndChosen == null;
        bool isOnly2ndInteractableChosen = interactable1stChosen == null && interactable2ndChosen != null;
        bool noInteractableIsChosen = interactable1stChosen == null && interactable2ndChosen == null;

        if (isOnly1stInteractableChosen)
        {
            //Debug.Log("NoInteractableNear and only 1st is chosen");
            StopVFXForDistantInteractable(interactable1stChosen);
        }
        else if (isOnly2ndInteractableChosen)
        {
            //Debug.Log("NoInteractableNear and only 2nd is chosen");
            StopVFXForDistantInteractable(interactable2ndChosen);
        }
        else if (noInteractableIsChosen) //no interactable is chosen, all vfx are stopped
        {
            //Debug.Log("NoInteractableNear and only none is chosen");
            StopChoosingVFX(choosingCircle1);
            StopChoosingVFX(choosingCircle2);
        }
    }

    //Decides to stop which vfx based on the one that is closer to the chosen one. The other one is stopped.
    private void StopVFXForDistantInteractable(Interactable7thPuzzleObject chosenInteractable)
    {
        float distanceToVFX1 = Vector3.Distance(chosenInteractable.transform.position, choosingCircle1.transform.position);
        float distanceToVFX2 = Vector3.Distance(chosenInteractable.transform.position, choosingCircle2.transform.position);

        if (Mathf.Abs(distanceToVFX1) > Mathf.Abs(distanceToVFX2)) //chosen one's vfx is closer to the chosen, so the other is stopped
        {
            StopChoosingVFX(choosingCircle1);
        }
        else
        {
            StopChoosingVFX(choosingCircle2);
        }
    }

    protected void ObjectCollidersApproached_PlayerInteractionManager7thPuzzle(object sender, List<Collider> colliderList)
    {
        List<Interactable7thPuzzleObject> interactableObjects = GetNearInteractablesList(colliderList);
        DetectInteractableApproached(interactableObjects);
    }

    protected virtual List<Interactable7thPuzzleObject> GetNearInteractablesList(List<Collider> colliderList)
    {
        List<Interactable7thPuzzleObject> interactableObjects = new();

        foreach (Collider collider in colliderList)
        {
            if (collider != null)
            {
                GameObject hitObject = collider.gameObject;

                //Making sure the object is an interactable one
                if (hitObject.TryGetComponent<Interactable7thPuzzleObject>(out var interactableObject))
                {
                    interactableObjects.Add(interactableObject);
                }
            }
        }

        return interactableObjects;
    }

    protected virtual void DetectInteractableApproached(List<Interactable7thPuzzleObject> interactableObjects)
    {
        switch (interactableObjects.Count)
        {
            case 1:
                Interactable7thPuzzleObject interactable = interactableObjects[0];
                OnInteractableApproached?.Invoke(this, interactable);
                break;
            default: //HANDLE LATER
                SetApproachedInteractable(null);
                break;
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
        swapKey = KeyCode.K;
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
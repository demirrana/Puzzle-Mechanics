using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager7thPuzzle : InteractionManager<IInteractableBehaviour7thPuzzle>
{
    #region Singleton and Events
    public static InteractionManager7thPuzzle Instance { get; private set; }

    public event EventHandler<KeyCode> OnBothInteractablesAreChosen;
    public event EventHandler OnAnyInteractableIsDeselected;
    public event EventHandler OnSwapKeyPressed;
    #endregion

    #region Fields & Serialized Fields
    [SerializeField] private ParticleSystem choosingCircle1;
    [SerializeField] private ParticleSystem choosingCircle2;

    private InteractionPanelMovable InteractionPanelIndividual;
    private Interactable7thPuzzleObject interactable1stChosen;
    private Interactable7thPuzzleObject interactable2ndChosen; //2nd interactable chosen to swap with the one in hand
    private Interactable<IInteractableBehaviour7thPuzzle> approachedInteractable;
    private KeyCode swapKey;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        SetInstance();
    }

    protected override void Start()
    {
        base.Start();
        InitializeInstances();
        OnObjectCollidersApproached += PlayerInteractionManager_ObjectCollidersApproached;
        OnInteractableApproached += PlayerInteractionManager_InteractableApproached;
        OnNoInteractableNear += PlayerInteractionManager_NoInteractableNear;
    }

    private void Update()
    {
        //if (approachedInteractable != null)
        //approachedInteractable.LogBehaviours();
        DetectInteractionConditionsMet();
    }

    private void OnDestroy()
    {
        OnObjectCollidersApproached -= PlayerInteractionManager_ObjectCollidersApproached;
        OnInteractableApproached -= PlayerInteractionManager_InteractableApproached;
        OnNoInteractableNear -= PlayerInteractionManager_NoInteractableNear;
    }
    #endregion

    #region Initialization and Setup
    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void InitializeInstances()
    {
        interactable1stChosen = interactableInHand as Interactable7thPuzzleObject;
        InteractionPanelIndividual = InteractionPanelMovable.Instance;
        interactable2ndChosen = null;
        approachedInteractable = null;
        swapKey = KeyCode.K;
    }
    #endregion

    #region Selection & Deselection Logic
    public void ChooseInteractable(Interactable7thPuzzleObject interactable) //is used by behaviour classes' Interact methods
    {
        if (interactable1stChosen == null) //chosen obj is 1st one
        {
            interactable1stChosen = interactable;
        }
        else //there is already a chosen obj, so the new one is the 2nd one
        {
            interactable2ndChosen = interactable;
        }

        if (interactable2ndChosen != null) //swap between interactables is possible
        {
            Debug.Log("2nd interactable is chosen. Swap key is: " + swapKey.ToString());
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

    private void SetApproachedInteractable(Interactable<IInteractableBehaviour7thPuzzle> interactable)
    {
        approachedInteractable = interactable;
    }
    #endregion

    #region Interaction Detection State Machine
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
        else //possible 2nd interactable to be chosen
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
            OnSwapKeyPressed?.Invoke(this, EventArgs.Empty);
            StartCoroutine(ApplySwappingBetweenObjects());
        }
    }

    private bool IsSwapKeyPressed()
    {
        return Input.GetKeyDown(swapKey);
    }
    #endregion

    #region Object Swapping and Animation Coroutines
    private IEnumerator ApplySwappingBetweenObjects()
    {
        Interactable7thPuzzleObject interactable1stHolder = interactable1stChosen;
        Interactable7thPuzzleObject interactable2ndHolder = interactable2ndChosen;
        DeselectInteractable(interactable2ndChosen); //Panel is hidden (HANDLE LATER to call only hiding panel at the start of method)
        DeselectInteractable(interactable1stChosen);

        Vector3 movingInY = new(0f, 2f, 0f); //the amount of moving in y axis toward the air
        float movingDuration = 1.2f;

        Vector3 interactable1OriginalPos = interactable1stHolder.transform.position;
        Vector3 interactable2OriginalPos = interactable2ndHolder.transform.position;

        Vector3 interactable1Pos = interactable1stHolder.transform.position;
        Vector3 interactable2Pos = interactable2ndHolder.transform.position;

        //moving into the air from floor
        yield return MoveTo(interactable1stHolder.transform, interactable2ndHolder.transform, interactable1Pos + movingInY, interactable2Pos + movingInY, movingDuration);

        Coroutine fade1 = StartCoroutine(interactable1stHolder.FadeOut());
        Coroutine fade2 = StartCoroutine(interactable2ndHolder.FadeOut());

        //fade out is completed before swapping the positions of the interactables in the air
        yield return fade1;
        yield return fade2;

        interactable1Pos = interactable1stHolder.transform.position;
        interactable2Pos = interactable2ndHolder.transform.position;
        interactable1stHolder.transform.position = interactable2Pos; //swapping in the air
        interactable2ndHolder.transform.position = interactable1Pos;
        Vector3 targetInteractable1Pos = new(interactable2Pos.x, interactable1OriginalPos.y, interactable2Pos.z);
        Vector3 targetInteractable2Pos = new(interactable1Pos.x, interactable2OriginalPos.y, interactable1Pos.z);
        
        //fading in the swapped interactables in the air
        Coroutine fadeIn1 = StartCoroutine(interactable1stHolder.FadeIn());
        Coroutine fadeIn2 = StartCoroutine(interactable2ndHolder.FadeIn());

        yield return fadeIn1;
        yield return fadeIn2;

        //moving interactable objects to the ground from the air
        yield return MoveTo(interactable1stHolder.transform, interactable2ndHolder.transform, targetInteractable1Pos, targetInteractable2Pos, movingDuration);

        interactable1stHolder.ResetBehaviours(); //goes back to the initial behaviour list it had at the beginning
        interactable2ndHolder.ResetBehaviours();

        //Debug.Log("Swapping took place and vfx are stopped.");
        StopChoosingVFX(choosingCircle1);
        StopChoosingVFX(choosingCircle2);
    }

    //Coroutine to move 2 objects simultaneously to their target positions in a given duration
    private IEnumerator MoveTo(Transform obj1, Transform obj2, Vector3 target1, Vector3 target2, float duration)
    {
        Vector3 start1 = obj1.position;
        Vector3 start2 = obj2.position;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            obj1.position = Vector3.Lerp(start1, target1, t);
            obj2.position = Vector3.Lerp(start2, target2, t);
            yield return null;
        }
    }
    #endregion
    
    #region Event Handlers
    protected override void PlayerInteractionManager_NoInteractableNear(object sender, EventArgs e)
    {
        DeactivateInteractionPanel();
        SetApproachedInteractable(null);
        ManageStoppingChoosingVFX_NoInteractableNear();
    }

    protected override void PlayerInteractionManager_InteractableApproached(object sender, Interactable<IInteractableBehaviour7thPuzzle> interactable)
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
    #endregion

    #region UI Panel Management
    private void ActivateInteractionPanel(object sender, Transform targetTransform, KeyCode interactionKey)
    {
        InteractionPanelIndividual.RaiseInteractionPanelActivated(sender, targetTransform, interactionKey);
    }

    private void DeactivateInteractionPanel()
    {
        InteractionPanelIndividual.RaiseInteractionPanelDeactivated(this);
    }
    #endregion

    #region Visual Effects Management
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

    //Stops the vfx of the interactable that is not chosen, if any of them is chosen. If none is chosen, all vfx are stopped.
    //(Because it is not known which vfx is played for the chosen interatable, it is detected with the distance to the chosen interactable.)
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
    #endregion
}
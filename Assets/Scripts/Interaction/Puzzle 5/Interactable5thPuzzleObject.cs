using System;
using UnityEngine;

public class Interactable5thPuzzleObject : Interactable5thPuzzle
{
    public event EventHandler<Interactable5thPuzzleState> OnInteractableStateChanged; //Might as well be in the Interactable
    public event EventHandler OnInteractableInHand;

    public enum Interactable5thPuzzleState
    {
        InHand,
        OnFloor,
        OnTable,
        LoadingTableView,
        ExittingTableView
    }

    private Interactable5thPuzzleState currentState;

    private void Awake()
    {
        currentState = Interactable5thPuzzleState.OnFloor;
        behavioursList.Add(new InteractableBehaviourPickUpFromFloor());
    }

    protected override void Start()
    {
        //Debug.Log($"[Child Start] {name} subscribing to OnInteractableStateChanged. InstanceID={GetInstanceID()}");
        base.Start();
        //OnInteractableStateChanged += Interactable5thPuzzle_OnInteractableStateChanged;
    }

    //Interact with the object, update its state and behaviours
    protected override void GetInteracted_Interactable(object sender, IInteractableBehaviour5thPuzzle interactionBehaviour)
    {
        interactionBehaviour.Interact<IInteractableBehaviour5thPuzzle>(this); //update player's hand state (empty or holding an object)
        Vector3 newTargetPosition = interactionBehaviour.GetTargetPosition();
        Transform newParentTransform = interactionBehaviour.GetNewParent();
        UpdateState(newTargetPosition, newParentTransform);
        UpdateBehavioursAfter<IInteractableBehaviour5thPuzzle>(interactionBehaviour);
    }

    public void UpdateState(Vector3 newPosition, Transform newParent)
    {
        transform.parent = null; //makes parent null before updating position
        transform.position = newPosition;
        transform.parent = newParent;
        //OnInteractableStateChanged?.Invoke(this, newState);
    }
}

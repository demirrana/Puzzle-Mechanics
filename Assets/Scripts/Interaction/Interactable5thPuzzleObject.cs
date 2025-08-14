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

    protected override void GetInteracted_Interactable(object sender, IInteractableBehaviour5thPuzzle interactionBehaviour)
    {
        Debug.Log("Before update behaviours:");
        LogBehaviours();
        //Debug.Log("GetInteracted_Interactable of Interactable5thPuzzle is called.");
        interactionBehaviour.Interact<IInteractableBehaviour5thPuzzle>(this);
        //Debug.Log("Behaviour keycode: " + interactionBehaviour.InteractionKeyCode.ToString());
        Vector3 newTargetPosition = interactionBehaviour.GetTargetPosition();
        Transform newParentTransform = interactionBehaviour.GetNewParent();
        UpdateState(newTargetPosition, newParentTransform);
        UpdateBehavioursAfter<IInteractableBehaviour5thPuzzle>(interactionBehaviour);
        Debug.Log("Updated behaviours:");
        LogBehaviours();
    }

    public void UpdateState(Vector3 newPosition, Transform newParent)
    {
        transform.position = newPosition;
        transform.parent = newParent;
        //OnInteractableStateChanged?.Invoke(this, newState);
    }
}

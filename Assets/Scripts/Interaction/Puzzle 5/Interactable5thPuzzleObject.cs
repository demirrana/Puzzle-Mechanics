using System;
using UnityEngine;

public class Interactable5thPuzzleObject : Interactable5thPuzzle
{
    public event EventHandler OnInteractableInHand;

    private void Awake()
    {
        behavioursList.Add(new InteractableBehaviourPickUpFromFloor());
    }

    protected override void Start()
    {
        base.Start();
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

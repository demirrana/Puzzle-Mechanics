using UnityEngine;

public class Interactable0thPuzzleObject : Interactable0thPuzzle
{
    public SOBookData bookData;
    [SerializeField] private Transform bookPlaceOnShelfTransform;

    private void Awake()
    {
        behavioursList.Add(new InteractableBehaviourPickUpFromShelf());
    }

    public Vector3 GetBookPlaceOnShelf()
    {
        return bookPlaceOnShelfTransform.position;
    }

    protected override void GetInteracted_Interactable(object sender, IInteractableBehaviour0thPuzzle interactionBehaviour)
    {
        interactionBehaviour.Interact<IInteractableBehaviour0thPuzzle>(this);
        Vector3 newTargetPosition = interactionBehaviour.GetTargetPosition(this);
        Transform newParentTransform = interactionBehaviour.GetNewParent();
        UpdateState(newTargetPosition, newParentTransform);
        UpdateBehavioursAfter<IInteractableBehaviour0thPuzzle>(interactionBehaviour);
    }

    public void UpdateState(Vector3 newPosition, Transform newParent)
    {
        transform.parent = null; //makes parent null before updating position
        transform.position = newPosition;
        transform.parent = newParent;
        //OnInteractableStateChanged?.Invoke(this, newState);
    }
}

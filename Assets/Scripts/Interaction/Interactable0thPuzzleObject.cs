using UnityEngine;

public class Interactable0thPuzzleObject : Interactable0thPuzzle
{
    public SOBookData bookData;
    [SerializeField] private Vector3 bookPlaceOnShelf;

    private void Awake()
    {
        behavioursList.Add(new InteractableBehaviourPickUpFromShelf());
    }

    public Vector3 GetBookPlaceOnShelf()
    {
        return bookPlaceOnShelf;
    }

    protected override void GetInteracted_Interactable(object sender, IInteractableBehaviour0thPuzzle interactionBehaviour)
    {
        interactionBehaviour.Interact<IInteractableBehaviour0thPuzzle>(this);
        Vector3 newTargetPosition = interactionBehaviour.GetTargetPosition(this);
        Transform newParentTransform = interactionBehaviour.GetNewParent();
        UpdateState(newTargetPosition, newParentTransform);
        UpdateBehavioursAfter<IInteractableBehaviour0thPuzzle>(interactionBehaviour);
    }

}

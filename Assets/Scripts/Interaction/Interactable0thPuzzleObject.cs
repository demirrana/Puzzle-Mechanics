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

    public Transform GetBookPlaceOnShelfTransform()
    {
        return bookPlaceOnShelfTransform;
    }

    {
        transform.position = newPosition;
    }

    public void SetParent(Transform parentTransform)
    {
        transform.parent = parentTransform;
    }

    protected override void GetInteracted_Interactable(object sender, IInteractableBehaviour0thPuzzle interactionBehaviour)
    {
        interactionBehaviour.Interact<IInteractableBehaviour0thPuzzle>(this);
        UpdateBehavioursAfter<IInteractableBehaviour0thPuzzle>(interactionBehaviour);
    }

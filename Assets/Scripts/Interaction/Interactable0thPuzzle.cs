using UnityEngine;

public class Interactable0thPuzzle : Interactable<IInteractableBehaviour0thPuzzle> //To be changed into 0thPuzzle
{
    protected void UpdateBehavioursAfter<T>(T behaviour) where T : IInteractableBehaviour0thPuzzle
    {
        behavioursList = behaviour.GetNewBehaviours();
    }
}
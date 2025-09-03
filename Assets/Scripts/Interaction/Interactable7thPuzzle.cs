using UnityEngine;

public class Interactable7thPuzzle : Interactable<IInteractableBehaviour7thPuzzle>
{
    protected void UpdateBehavioursAfter<T>(T behaviour) where T : IInteractableBehaviour7thPuzzle
    {
        behavioursList = behaviour.GetNewBehaviours();
    }
}

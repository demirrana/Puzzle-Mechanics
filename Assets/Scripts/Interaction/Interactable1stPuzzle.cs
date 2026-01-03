using UnityEngine;

public class Interactable1stPuzzle : Interactable<IInteractableBehaviour1stPuzzle>
{
    protected void UpdateBehavioursAfter<T>(T behaviour) where T : IInteractableBehaviour1stPuzzle
    {
        behavioursList = behaviour.GetNewBehaviours();
    }
}
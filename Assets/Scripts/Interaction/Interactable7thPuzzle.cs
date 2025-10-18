using UnityEngine;

public class Interactable7thPuzzle : Interactable<IInteractableBehaviour7thPuzzle>
{
    protected void UpdateBehavioursAfter<T>(T behaviour) where T : IInteractableBehaviour7thPuzzle
    {
        //Debug.Log("UpdateBehavioursAfter");
        behavioursList = behaviour.GetNewBehaviours();
        //Debug.Log("New behaviour: " + behavioursList[0].ToString());
    }
}

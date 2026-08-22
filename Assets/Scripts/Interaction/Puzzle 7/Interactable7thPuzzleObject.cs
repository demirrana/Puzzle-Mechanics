using System.Collections.Generic;
using UnityEngine;

public class Interactable7thPuzzleObject : Interactable7thPuzzle
{
    private void Awake()
    {
        behavioursList.Add(new InteractableBehaviourBeChosen());

        foreach (IInteractableBehaviour b in behavioursList)
        {
            //Debug.Log(b.ToString());
        }
    }

    public void LogBehaviours()
    {
        Debug.Log("Behaviours are:");
        foreach (IInteractableBehaviour7thPuzzle b in behavioursList)
        {
            Debug.Log(b.ToString());
        }
    }

    public void ResetBehaviours()
    {
        //Debug.Log("ResetBehaviours");
        List<IInteractableBehaviour7thPuzzle> initialBehavioursList = new();
        initialBehavioursList.Add(new InteractableBehaviourBeChosen());
        behavioursList = initialBehavioursList;
    }

    protected override void GetInteracted_Interactable(object sender, IInteractableBehaviour7thPuzzle interactionBehaviour)
    {
        //Debug.Log("Interact performed.");
        interactionBehaviour.Interact<IInteractableBehaviour7thPuzzle>(this);
        UpdateBehavioursAfter(interactionBehaviour);
    }
}

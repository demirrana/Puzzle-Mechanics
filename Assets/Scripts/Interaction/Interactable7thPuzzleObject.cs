using System.Collections.Generic;
using UnityEngine;

public class Interactable7thPuzzleObject : Interactable7thPuzzle
{
    public void ResetBehaviours()
    {
        List<IInteractableBehaviour7thPuzzle> initialBehavioursList = new();
        initialBehavioursList.Add(new InteractableBehaviourBeChosen());
        behavioursList = initialBehavioursList;
    }

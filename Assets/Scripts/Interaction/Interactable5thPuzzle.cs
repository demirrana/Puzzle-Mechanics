using System;
using System.Collections.Generic;
using UnityEngine;

public class Interactable5thPuzzle : Interactable<IInteractableBehaviour5thPuzzle>
{
    protected void UpdateBehavioursAfter<T>(T behaviour) where T : IInteractableBehaviour5thPuzzle
    {
        behavioursList = behaviour.GetNewBehaviours();
    }
}
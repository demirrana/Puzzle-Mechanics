using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable5thPuzzle : Interactable<IInteractableBehaviour5thPuzzle>
{
    //Update possible next behaviours after the interaction is applied
    protected void UpdateBehavioursAfter<T>(T behaviour) where T : IInteractableBehaviour5thPuzzle
    {
        behavioursList = behaviour.GetNewBehaviours();
    }

    protected void LogBehaviours()
    {
        foreach (IInteractableBehaviour5thPuzzle b in behavioursList)
        {
            Debug.Log(b.ToSafeString());
        }
    }
}
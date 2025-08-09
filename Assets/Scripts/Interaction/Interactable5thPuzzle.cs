using System;
using UnityEngine;

public class Interactable5thPuzzle : Interactable
{
    private void Awake()
    {
        behavioursList.Add(new InteractableBehaviourCollectFromFloor());
    }

    private void Start()
    {
    }

    override protected void GetInteracted_Interactable(object sender, IInteractionBehaviour e)
    {

    }
}
using System;
using UnityEngine;
public class Interactable1stPuzzleObject : Interactable1stPuzzle
{

    private void Awake()
    {
        behavioursList.Add(new InteractableBehaviourCollectKeyPart());
    }

    public void SetParent(Transform parentTransform)
    {
        transform.parent = parentTransform;
    }

}

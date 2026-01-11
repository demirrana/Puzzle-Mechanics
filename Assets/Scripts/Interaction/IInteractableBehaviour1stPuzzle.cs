using System.Collections.Generic;
using UnityEngine;

public class IInteractableBehaviour1stPuzzle : IInteractableBehaviour
{
    public virtual KeyCode InteractionKeyCode => KeyCode.E;

    public virtual void Interact<IInteractableBehaviour>(Interactable1stPuzzleObject interactable)
    {
        //Debug.Log("IInteractableBehaviour1stPuzzle has called Interact.");
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
    }

    public virtual List<IInteractableBehaviour1stPuzzle> GetNewBehaviours()
    {
        return null;
    }

    public virtual Vector3 GetTargetPosition(Interactable1stPuzzleObject interactable)
    {
        return Vector3.zero;
    }

    public virtual Transform GetNewParent()
    {
        return default;
    }
}

public class InteractableBehaviourCollectKeyPart : IInteractableBehaviour1stPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour1stPuzzle>(Interactable1stPuzzleObject interactable)
    {
        Debug.Log("Key part is collected from floor.");

        interactable.SetParent(null);
    }

    public override List<IInteractableBehaviour1stPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour1stPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourSnapToKeySocket());
        return newBehaviours;
    }

    public override Vector3 GetTargetPosition(Interactable1stPuzzleObject interactable)
    {
        return InteractionManager1stPuzzle.Instance.GetHandPosition();
    }

    public override Transform GetNewParent()
    {
        return InteractionManager1stPuzzle.Instance.GetHandTransform();
    }
}

public class InteractableBehaviourSnapToKeySocket : IInteractableBehaviour1stPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.Mouse0;
}

public class InteractableBehaviourUnsnapFromSocket : IInteractableBehaviour1stPuzzle
{
    
}
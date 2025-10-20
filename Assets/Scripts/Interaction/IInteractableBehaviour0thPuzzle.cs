using System.Collections.Generic;
using UnityEngine;

public class IInteractableBehaviour0thPuzzle : IInteractableBehaviour
{
    public virtual KeyCode InteractionKeyCode => KeyCode.E;

    public virtual void Interact<IInteractableBehaviour>(Interactable0thPuzzleObject interactable)
    {
        //Debug.Log("IInteractableBehaviour0thPuzzle has called Interact.");
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
    }

    public virtual List<IInteractableBehaviour0thPuzzle> GetNewBehaviours()
    {
        return null;
    }

    public virtual Vector3 GetTargetPosition(Interactable0thPuzzleObject interactable)
    {
        return Vector3.zero;
    }

    public virtual Transform GetNewParent()
    {
        return default;
    }
}

public class InteractableBehaviourPickUpFromShelf : IInteractableBehaviour0thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;


    public override List<IInteractableBehaviour0thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour0thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourPutOnShelf());
        newBehaviours.Add(new InteractableBehaviourPutOnBookPlatform());
        return newBehaviours;
    }

}

public class InteractableBehaviourPutOnShelf : IInteractableBehaviour0thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

}

public class InteractableBehaviourPutOnBookPlatform : IInteractableBehaviour0thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

}

public class InteractableBehaviourPickUpFromBookPlatform : IInteractableBehaviour0thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

}
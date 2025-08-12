using System.Collections.Generic;
using UnityEngine;

public class IInteractableBehaviour5thPuzzle : IInteractableBehaviour
{
    public virtual KeyCode InteractionKeyCode => KeyCode.E;

    public virtual void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        Debug.Log("IInteractableBehaviour5thPuzzle has called Interact.");
    }

    public virtual List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        return null;
    }
}

public class InteractableBehaviourPickUpFromFloor : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();

        newBehaviours.Add(new InteractableBehaviourDropOnFloor());
        newBehaviours.Add(new InteractableBehaviourDragOnTable());

        return newBehaviours;
    }
}

public class InteractableBehaviourPickUpFromTableToHand : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from table");

        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.ExittingTableView);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourDropOnFloor());
        newBehaviours.Add(new InteractableBehaviourDragOnTable());
        return newBehaviours;
    }
}

public class InteractableBehaviourDropOnFloor : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.F;

    public override void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Drop on floor");

        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.OnFloor);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourPickUpFromFloor());
        return newBehaviours;
    }
}

public class InteractableBehaviourPutOnTableSlot : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Drop on table");

        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.LoadingTableView);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourDragOnTable());
        return newBehaviours;
    }
}

public class InteractableBehaviourDragOnTable : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Drop on table");

        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.LoadingTableView);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourPickUpFromTableToHand());
        newBehaviours.Add(new InteractableBehaviourPutOnTableSlot());
        return newBehaviours;
    }
}

public class InteractableBehaviourOpenTableView : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourCloseTableView());
        return newBehaviours;
    }
}

public class InteractableBehaviourCloseTableView : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourOpenTableView());
        return newBehaviours;
    }
}

public class InteractableBehaviourFullSlot : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourEmptySlot());
        return newBehaviours;
    }
}

public class InteractableBehaviourEmptySlot : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour>(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourFullSlot());
        return newBehaviours;
    }
}
using UnityEngine;

public class IInteractableBehaviour5thPuzzle : IInteractableBehaviour<Interactable5thPuzzle>
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    void IInteractableBehaviour.Interact(Interactable interactable)
    {
        Interact(interactable as Interactable5thPuzzle);
    }

    public virtual void Interact(Interactable5thPuzzle interactable)
    {
        Debug.Log("IInteractableBehaviour5thPuzzle has called Interact.");
    }
}

public class InteractableBehaviourCollectFromFloor : IInteractableBehaviour5thPuzzle
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }
}

public class InteractableBehaviourCollectFromTable : IInteractableBehaviour5thPuzzle
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from table");

        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.ExittingTableView);
    }
}

public class InteractableBehaviourDropOnFloor : IInteractableBehaviour5thPuzzle
{
    public KeyCode InteractionKeyCode => KeyCode.F;

    public override void Interact(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Drop on floor");

        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.OnFloor);
    }
}

public class InteractableBehaviourDropOnTable : IInteractableBehaviour5thPuzzle
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Drop on table");

        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.LoadingTableView);
    }
}

public class InteractableBehaviourPickUpFromTable : IInteractableBehaviour5thPuzzle
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Drop on table");

        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.LoadingTableView);
    }
}

public class InteractableBehaviourOpenTableView : IInteractableBehaviour5thPuzzle
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }
}

public class InteractableBehaviourCloseTableView : IInteractableBehaviour5thPuzzle
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }
}

public class InteractableBehaviourFullSlot : IInteractableBehaviour5thPuzzle
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }
}

public class InteractableBehaviourEmptySlot : IInteractableBehaviour5thPuzzle
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact(Interactable5thPuzzle interactable)
    {
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }
}
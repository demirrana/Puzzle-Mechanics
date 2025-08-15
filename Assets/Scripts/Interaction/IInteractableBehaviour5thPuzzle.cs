using System.Collections.Generic;
using UnityEngine;

public class IInteractableBehaviour5thPuzzle : IInteractableBehaviour
{
    public virtual KeyCode InteractionKeyCode => KeyCode.E;

    public virtual void Interact<IInteractableBehaviour>(Interactable5thPuzzleObject interactable)
    {
        Debug.Log("IInteractableBehaviour5thPuzzle has called Interact.");
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
    }

    public virtual List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        return null;
    }

    public virtual Vector3 GetTargetPosition()
    {
        return Vector3.zero;
    }

    public virtual Transform GetNewParent()
    {
        return InteractionManager5thPuzzle.Instance.GetObjectsHolderTransform();
    }
}

public class InteractableBehaviourPickUpFromFloor : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour5thPuzzle>(Interactable5thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour5thPuzzle>(interactable);
        Debug.Log("PickUpFromFloor interact method is called.");
        InteractionManager5thPuzzle.Instance.RaiseInteractableInHandChanged(interactable);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourDropOnFloor());
        newBehaviours.Add(new InteractableBehaviourDragOnTableFromHand());
        return newBehaviours;
    }

    public override Vector3 GetTargetPosition()
    {
        return InteractionManager5thPuzzle.Instance.GetHandPosition();
    }

    public override Transform GetNewParent()
    {
        return InteractionManager5thPuzzle.Instance.GetHandTransform();
    }
}

public class InteractableBehaviourPickUpFromTableToHand : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.F;

    public override void Interact<IInteractableBehaviour5thPuzzle>(Interactable5thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour5thPuzzle>(interactable);
        Debug.Log("PickUpFromTableToHand's interact method is called.");
        Interactable5thPuzzleTable.Instance.RaiseTableViewDeactivated(); //game view is activated
        //interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.ExittingTableView);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourDropOnFloor());
        newBehaviours.Add(new InteractableBehaviourDragOnTableFromHand());
        return newBehaviours;
    }

    public override Vector3 GetTargetPosition()
    {
        return InteractionManager5thPuzzle.Instance.GetHandPosition();
    }

    public override Transform GetNewParent()
    {
        return InteractionManager5thPuzzle.Instance.GetHandTransform();
    }
}

public class InteractableBehaviourDropOnFloor : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.F;

    public override void Interact<IInteractableBehaviour5thPuzzle>(Interactable5thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour5thPuzzle>(interactable);
        Debug.Log("Drop on floor's interact method is called");
        InteractionManager5thPuzzle.Instance.RaiseInteractableInHandChanged(null);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourPickUpFromFloor());
        return newBehaviours;
    }

    public override Vector3 GetTargetPosition()
    {
        Vector3 handPosition = InteractionManager5thPuzzle.Instance.GetHandPosition();
        return new Vector3(handPosition.x, 0f, handPosition.z + 1f); //will be changed to find the nearest uncolliding position
    }
}

public class InteractableBehaviourPutOnTableSlot : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour5thPuzzle>(Interactable5thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour5thPuzzle>(interactable);
        Debug.Log("PutOnTableSlot's interact method is called.");
        InteractionManager5thPuzzle.Instance.RaiseInteractableInHandChanged(null);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourDragOnTableFromSlot());
        return newBehaviours;
    }

    //Might be changed since GetPointedEmptySlot is called twice in two different scopes
    public override Vector3 GetTargetPosition()
    {
        return Interactable5thPuzzleTable.Instance.GetPointedEmptySlot().transform.position;
    }
}

public class InteractableBehaviourDragOnTableFromHand : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour5thPuzzle>(Interactable5thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour5thPuzzle>(interactable);
        Debug.Log("DragOnTableFromHand's interact method is called.");
        
        Interactable5thPuzzleTable.Instance.RaiseTableViewActivated(); //table view activated
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourPickUpFromTableToHand());
        newBehaviours.Add(new InteractableBehaviourPutOnTableSlot());
        return newBehaviours;
    }

    //After this, object should be following the position of the mouse
    public override Vector3 GetTargetPosition()
    {
        CameraManager cameraManager = CameraManager.Instance;
        return cameraManager.GetActiveCamera().transform.position - Vector3.up;
    }
}

public class InteractableBehaviourDragOnTableFromSlot : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour5thPuzzle>(Interactable5thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour5thPuzzle>(interactable);
        Debug.Log("DragOnTableFromSlot interact method is called.");
        InteractionManager5thPuzzle.Instance.RaiseInteractableInHandChanged(interactable);
    }

    public override List<IInteractableBehaviour5thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour5thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourPickUpFromTableToHand());
        newBehaviours.Add(new InteractableBehaviourPutOnTableSlot());
        return newBehaviours;
    }

    //After this, object should be following the position of the mouse
    public override Vector3 GetTargetPosition()
    {
        CameraManager cameraManager = CameraManager.Instance;
        return cameraManager.GetActiveCamera().transform.position - Vector3.up;
    }
}
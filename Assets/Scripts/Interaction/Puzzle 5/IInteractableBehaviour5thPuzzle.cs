using System.Collections.Generic;
using UnityEngine;

public class IInteractableBehaviour5thPuzzle : IInteractableBehaviour
{
    public virtual KeyCode InteractionKeyCode => KeyCode.E;

    //interaction is applied through this method, which is called in the InteractionManager when the interaction conditions are met
    public virtual void Interact<IInteractableBehaviour>(Interactable5thPuzzleObject interactable) 
    {
        //Debug.Log("IInteractableBehaviour5thPuzzle has called Interact.");
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
    }

    public virtual List<IInteractableBehaviour5thPuzzle> GetNewBehaviours() //potential next behaviour list is updated after the interaction
    {
        return null;
    }

    public virtual Vector3 GetTargetPosition() //the position object will be moved after the interaction
    {
        return Vector3.zero;
    }

    public virtual Transform GetNewParent() //the parent object may be changed after the interaction
    {
        return InteractionManager5thPuzzle.Instance.GetObjectsHolderTransform();
    }
}

//Behaviour classes for the 5th puzzle

//Pick up object from floor
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

//Pick up object from table (dragging) to hand
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

//Drop object on floor from hand
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
        float dropRadius = InteractionManager5thPuzzle.Instance.GetDropRadius();
        return InteractionManager5thPuzzle.Instance.GetNearestPosCollidingWithNothing(dropRadius);
    }
}

//Put object on table slot from dragging on table
public class InteractableBehaviourPutOnTableSlot : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour5thPuzzle>(Interactable5thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour5thPuzzle>(interactable);
        Debug.Log("PutOnTableSlot's interact method is called.");
        Interactable5thPuzzleTableSlot slot = InteractionManager5thPuzzle.Instance.GetPointedSlotBeforeKeyPress();
        slot.SetInteractableInSlot(interactable); //slot is now full
        Interactable5thPuzzleTable.Instance.TransferSlotToFullSlots(slot); //slot is added to fullSlots
        InteractionManager5thPuzzle.Instance.RaiseInteractableInHandChanged(null); //hand is empty now
        //Interactable5thPuzzleTable.Instance.LogEmptyAndFullSlots(); //to be deleted
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
        return InteractionManager5thPuzzle.Instance.GetPointedSlotBeforeKeyPress().transform.position;
    }
}

//Drag object on table from hand
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
        return MouseManager.Instance.GetMousePositionInWorld();
    }
}

//Drag object on table by picking it up from table slot
public class InteractableBehaviourDragOnTableFromSlot : IInteractableBehaviour5thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour5thPuzzle>(Interactable5thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour5thPuzzle>(interactable);
        Debug.Log("DragOnTableFromSlot interact method is called.");
        Interactable5thPuzzleTableSlot slot = InteractionManager5thPuzzle.Instance.GetPointedSlotBeforeKeyPress();
        slot.SetInteractableInSlot(null); //slot is empty now
        Interactable5thPuzzleTable.Instance.TransferSlotToEmptySlots(slot); //slot is added to emptySlots
        InteractionManager5thPuzzle.Instance.RaiseInteractableInHandChanged(interactable); //object is in hand
        //Interactable5thPuzzleTable.Instance.LogEmptyAndFullSlots(); //to be deleted
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
        return MouseManager.Instance.GetMousePositionInWorld();
    }
}
using System.Collections.Generic;
using UnityEngine;

public class IInteractableBehaviour7thPuzzle : IInteractableBehaviour
{
    public virtual KeyCode InteractionKeyCode => KeyCode.E;

    public virtual void Interact<IInteractableBehaviour>(Interactable7thPuzzleObject interactable)
    {
        //Debug.Log("IInteractableBehaviour5thPuzzle has called Interact.");
        if (interactable == null)
        {
            Debug.LogWarning("Wrong type of interactable is found!");
            return;
        }
    }

    public virtual List<IInteractableBehaviour7thPuzzle> GetNewBehaviours()
    {
        return null;
    }
}

public class InteractableBehaviourBeChosen : IInteractableBehaviour7thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour7thPuzzle>(Interactable7thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour7thPuzzle>(interactable);
        InteractionManager7thPuzzle.Instance.ChooseInteractable(interactable);
        Debug.Log("BeChosen interact method is called.");
    }

    public override List<IInteractableBehaviour7thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour7thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourBeDeselected());
        return newBehaviours;
    }
}

public class InteractableBehaviourBeDeselected : IInteractableBehaviour7thPuzzle
{
    public override KeyCode InteractionKeyCode => KeyCode.E;

    public override void Interact<IInteractableBehaviour7thPuzzle>(Interactable7thPuzzleObject interactable)
    {
        base.Interact<IInteractableBehaviour7thPuzzle>(interactable);
        InteractionManager7thPuzzle.Instance.DeselectInteractable(interactable);
        Debug.Log("BeDeselected interact method is called.");
    }

    public override List<IInteractableBehaviour7thPuzzle> GetNewBehaviours()
    {
        List<IInteractableBehaviour7thPuzzle> newBehaviours = new();
        newBehaviours.Add(new InteractableBehaviourBeChosen());
        return newBehaviours;
    }
}
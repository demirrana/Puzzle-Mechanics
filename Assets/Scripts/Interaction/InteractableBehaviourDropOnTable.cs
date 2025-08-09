using UnityEngine;

public class InteractableBehaviourDropOnTable : IInteractionBehaviour<Interactable5thPuzzle>
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    void IInteractionBehaviour.Interact(Interactable interactable)
    {
        Interact(interactable as Interactable5thPuzzle);
    }

    public void Interact(Interactable5thPuzzle interactable)
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
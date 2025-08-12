using UnityEngine;

public interface IInteractableBehaviour
{
    KeyCode InteractionKeyCode { get; }
    void Interact(Interactable interactable) { }
}

public interface IInteractableBehaviour<T> : IInteractableBehaviour where T : Interactable
{
    void Interact(T interactable) { }
}

public class InteractableBehaviourTable : IInteractableBehaviour<Interactable5thPuzzle>
{
    public KeyCode InteractionKeyCode => KeyCode.E;

    void IInteractableBehaviour.Interact(Interactable interactable)
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
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }
}
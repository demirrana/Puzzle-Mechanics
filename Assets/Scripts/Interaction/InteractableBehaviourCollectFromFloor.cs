using UnityEngine;

public class InteractableBehaviourCollectFromFloor : IInteractionBehaviour<Interactable5thPuzzle>
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
        Debug.Log("Collect from floor interact method is called.");
        //interactable.UpdatePosition(PlayerInteractionManager.Instance.gameObject.transform);
        interactable.UpdateState(Interactable5thPuzzle.Interactable5thPuzzleState.InHand);
    }
}
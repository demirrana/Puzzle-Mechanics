using UnityEngine;

public class Interactable5thPuzzleTable : Interactable
{
    protected override void GetInteracted_Interactable(object sender, IInteractionBehaviour interactionBehaviour)
    {
        //Debug.Log($"[Child Handler] invoked on {name}, target type: {GetType().Name} (InstanceID {GetInstanceID()})");
        Debug.Log("GetInteracted_Interactable of Interactable5thPuzzleTable is called.");
        interactionBehaviour.Interact(this);
        Debug.Log("Behaviour keycode: " + interactionBehaviour.InteractionKeyCode.ToString());
    }   
}
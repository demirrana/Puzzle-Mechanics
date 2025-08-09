using UnityEngine;

public interface IInteractionBehaviour
{
    KeyCode InteractionKeyCode { get; }
    void Interact(Interactable interactable) { }
}

public interface IInteractionBehaviour<T> : IInteractionBehaviour where T : Interactable
{
    void Interact(T interactable) {}
}
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
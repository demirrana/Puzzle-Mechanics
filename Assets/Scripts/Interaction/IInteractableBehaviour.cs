using UnityEngine;

public interface IInteractableBehaviour
{
    KeyCode InteractionKeyCode { get; }
    void Interact<T>(Interactable<T> interactable) where T : IInteractableBehaviour { }
}
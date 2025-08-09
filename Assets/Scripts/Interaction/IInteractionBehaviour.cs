using UnityEngine;

public interface IInteractionBehaviour
{
    KeyCode InteractionKeyCode { get; }
    void Interact() {}
}
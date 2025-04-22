using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event EventHandler OnGotInteracted;

    private void Start()
    {
        PlayerInteractionManager.Instance.OnInteractableInteracted += InteractableInteracted_PlayerInteractionManager;
        OnGotInteracted += GetInteracted_Interactable;
    }

    public bool CanObjectBeInteracted()
    {
        return true;
    }

    protected virtual void InteractableInteracted_PlayerInteractionManager(object sender, Interactable interactableObject)
    {
        OnGotInteracted?.Invoke(this, null);
    }

    public virtual void GetInteracted_Interactable(object sender, EventArgs e)
    {
        Debug.Log("This object is interacted by the player.");
    }
}

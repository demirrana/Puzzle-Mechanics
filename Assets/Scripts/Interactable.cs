using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event EventHandler OnGotInteracted;

    private void Start()
    {
        PlayerInteractionManager.Instance.OnAnyObjectInteracted += ObjectInteracted_PlayerInteractionManager;
        OnGotInteracted += GetInteracted;
    }

    public bool CanObjectBeInteracted()
    {
        return true;
    }

    protected virtual void ObjectInteracted_PlayerInteractionManager(object sender, Interactable interactableObject)
    {
        OnGotInteracted?.Invoke(this, null);
    }

    protected virtual void GetInteracted(object sender, EventArgs e)
    {
        Debug.Log("This object is interacted by the player.");
    }
}

using System;
using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event EventHandler OnGotInteracted;

    private void Start()
    {
        //Might be a problem since PlayerInteractionManager is disabled and enabled along the game!!!!!!!!
        PlayerInteractionManager.Instance.OnInteractableInteracted += InteractableInteracted_PlayerInteractionManager;
        OnGotInteracted += GetInteracted_Interactable;
    }

    public void GetInteracted()
    {
        OnGotInteracted?.Invoke(this, null);
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

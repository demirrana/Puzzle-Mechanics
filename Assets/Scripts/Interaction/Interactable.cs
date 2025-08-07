using System;
using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event EventHandler OnGotInteracted;

    protected virtual KeyCode InteractionKey1 => KeyCode.E;
    protected virtual KeyCode InteractionKey2 => KeyCode.F;

    private void Start()
    {
        //Might be a problem since PlayerInteractionManager is disabled and enabled along the game!!!!!!!!
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

    public KeyCode GetInteractionKey1()
    {
        return InteractionKey1;
    }

    public KeyCode GetInteractionKey2()
    {
        return InteractionKey2;
    }
    
    protected virtual void GetInteracted_Interactable(object sender, EventArgs e)
    {
        Debug.Log("This object is interacted by the player.");
    }
}
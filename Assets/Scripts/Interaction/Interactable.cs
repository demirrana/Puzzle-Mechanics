using System;
using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event EventHandler OnGotInteracted;

    public void GetInteracted()
    {
        OnGotInteracted?.Invoke(this, null);
    }

    public bool CanObjectBeInteracted()
    {
        return true;
    }    
}
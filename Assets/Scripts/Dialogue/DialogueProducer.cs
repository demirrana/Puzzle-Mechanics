using System;
using UnityEngine;

public class DialogueProducer : MonoBehaviour
{
    protected event EventHandler OnDialogueActivated;
    protected event EventHandler OnDialogueDeactivated;

    protected bool isDialogueActivated = false;

    private void Update()
    {
        HandleDialogues();   
    }

    protected virtual void HandleDialogues()
    {
        Debug.Log("HandleDialogues");
    }
}

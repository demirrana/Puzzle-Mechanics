using System;
using UnityEngine;

public class InteractableAnimated : Interactable
{
    private Animator animator;

    private bool toggledOn = false;
    private const String IsGoing = "IsGoing";
    private const String IsComing = "IsComing";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    override protected void GetInteracted_Interactable(object sender, IInteractionBehaviour e)
    {
        PlayerScriptsManager.Instance.DisableInteractionScript();
        Debug.Log("This object is interacted by the player and the interaction script is disabled.");

        if (!toggledOn)
        {
            animator.SetTrigger(IsGoing);
        }
        else
        {
            animator.SetTrigger(IsComing);
        }

        toggledOn = !toggledOn;
    }
}
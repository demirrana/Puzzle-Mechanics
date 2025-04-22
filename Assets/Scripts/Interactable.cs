using System;
using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event EventHandler OnGotInteracted;
    
    private Animator animator;

    private bool toggledOn = false;
    private String isGoing = "isGoing";
    private String isComing = "isComing";

    private void Awake()
    {
        animator = GetComponent<Animator>();   
    }

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

        if (!toggledOn)
        {
            animator.SetBool(isGoing, true);
            animator.SetBool(isComing, false);
        }
        else
        {
            animator.SetBool(isComing, true);
            animator.SetBool(isGoing, false);
        }

        toggledOn = !toggledOn;
    }
}

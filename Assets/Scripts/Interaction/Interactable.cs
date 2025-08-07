using System;
using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event EventHandler OnGotInteracted;
    
    private Animator animator;

    private bool toggledOn = false;
    private const String IsGoing = "IsGoing";
    private const String IsComing = "IsComing";

    private void Awake()
    {
        animator = GetComponent<Animator>();   
    }

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

    protected virtual void GetInteracted_Interactable(object sender, EventArgs e)
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

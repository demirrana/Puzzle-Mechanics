using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Player5thPuzzleInteractionManager : PlayerInteractionManager
{
    private Interactable interactableAtHand;

    private void Awake()
    {
        interactableAtHand = null;
    }

    override protected List<Interactable> GetNearInteractablesList(List<Collider> colliderList)
    {
        List<Interactable> interactableObjects = new();

        foreach (Collider collider in colliderList)
        {
            if (collider != null)
            {
                GameObject hitObject = collider.gameObject;
                Interactable[] interactableComponents = hitObject.GetComponents<Interactable>();

                if (interactableComponents.Length != 0)
                {
                    foreach (Interactable interactable in interactableComponents)
                    {
                        interactableObjects.Add(interactable);
                    }
                }
            }
        }

        return interactableObjects;
    }

    override protected void DetectAnInteractableApproached(List<Interactable> interactableObjects)
    {
        if (interactableAtHand == null)
        {
            switch (interactableObjects.Count)
            {
                case 0:
                    break;
                case 1:
                    Invoke_OnInteractableApproached(this, interactableObjects[0]);
                    break;
                //TO BE CHANGED IN THE FUTURE
                default:
                    Debug.Log("There are more than 1 interactables");
                    //TO BE CHANGED: When there are more than 1 interactables, the camera angle should decide which one to interact with
                    Invoke_OnInteractableApproached(this, interactableObjects[1]); //For now, the one after first interactable can be interacted 
                    break;
            }
        }
        else
        {
            foreach (Interactable interactable in interactableObjects)
            {
                if (interactable.GetType() == typeof(Interactable5thPuzzle)) //to be changed to table's type
                {
                    Invoke_OnInteractableApproached(this, interactable);
                    return;
                }
            }

            Invoke_OnInteractableApproached(this, interactableAtHand); //when player drops the object
        }
    }

    override protected bool IsInteractionKeyPressed(Interactable interactable)
    {
        if (interactableAtHand == null && Input.GetKeyDown(interactable.GetInteractionKey())) //to collect from the floor
        {
            Invoke_OnInteractionKeyPressed(this);
            return true;
        }
        else if (interactableAtHand != null)
        {
            if (interactable == interactableAtHand && Input.GetKeyDown(interactable.GetInteractionKey())) //to drop the object
            {
                Invoke_OnInteractionKeyPressed(this);
                return true;
            }
            else if (Input.GetKeyDown(interactable.GetInteractionKey())) //to put on the table
            {
                Invoke_OnInteractionKeyPressed(this);
                return true;
            }
        }

        return false;
    }
}
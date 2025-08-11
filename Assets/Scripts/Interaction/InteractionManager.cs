using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractionManager : MonoBehaviour
{

    public event EventHandler<Interactable> OnInteractionConditionsMet;
    public event EventHandler<InteractionBehaviourEventArgs> OnInteractionKeyPressed; //Invoked if an interactable is approached
    public event EventHandler<InteractionBehaviourEventArgs> OnInteractableInteracted; //for now, considered as the same with OnInteractionKeyPressed

    public class InteractionBehaviourEventArgs : EventArgs
    {
        public Interactable InteractedObject { get; }
        public IInteractionBehaviour InteractionBehaviour { get; }

        public InteractionBehaviourEventArgs(Interactable interactable, IInteractionBehaviour interactionBehaviour)
        {
            InteractedObject = interactable;
            InteractionBehaviour = interactionBehaviour;
        }
    }

    [SerializeField] private float proximityThreshold = 1f; //The minimum distance to an Interactable in order to detect it
    [SerializeField] private float playerHeight = 1.67f; //Can be moved to another script
    [SerializeField] private int rayCount = 5;
    [SerializeField] private Transform handTransform;


    {
    }

    {
    }

    {

    }

    {
    }

    {

    }

    {
        {
        }
    }

    {
    }

    //Creates a distinction between (key required for that interaction is pressed) and (interaction taking place)
    protected bool IsInteractedOnceKeyIsPressed()
    {
        return interactedOnceKeyIsPressed;
    }

    private bool IsInteractionKeyPressed(KeyCode interactionKeyCode)
    {
        if (Input.GetKeyDown(interactionKeyCode))
        {
            //OnInteractionKeyPressed?.Invoke(this, null); !!!!could be on the extending classes
            return true;
        }

        return false;
    }
}

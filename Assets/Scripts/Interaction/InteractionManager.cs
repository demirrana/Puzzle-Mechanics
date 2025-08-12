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

    [SerializeField] protected float proximityThreshold = 1f; //The minimum distance to an Interactable in order to detect it
    [SerializeField] protected float playerHeight = 1.67f; //Can be moved to another script
    [SerializeField] protected int rayCount = 5;
    [SerializeField] private Transform handTransform;

    protected Interactable interactableInHand = null;
    private bool interactedOnceKeyIsPressed = true;

    protected virtual void Start()
    {
        OnInteractionConditionsMet += InteractionManager_InteractionConditionsMet;
        OnInteractionKeyPressed += InteractionManager_InteractionKeyPressed;
        OnInteractableInteracted += InteractionManager_InteractableInteracted;
    }

    protected virtual void DetectInteractionConditionsMet()
    {
        Debug.Log("Base class called DetectWhenInteractionConditionsMet");
    }

    protected virtual void InteractionManager_InteractionConditionsMet(object sender, Interactable interactable)
    {
        DetectBehavioursApplied(interactable);
    }

    protected virtual void InteractionManager_InteractionKeyPressed(object sender, InteractionBehaviourEventArgs e)
    {
        if (IsInteractedOnceKeyIsPressed())
        {
            OnInteractableInteracted?.Invoke(this, e);
        }
        else
        {
            //another condition
        }
    }

    protected virtual void InteractionManager_InteractableInteracted(object sender, InteractionBehaviourEventArgs e)
    {
        e.InteractedObject.GetInteracted(e.InteractionBehaviour);
    }

    protected void DetectBehavioursApplied(Interactable interactable)
    {
        foreach (IInteractionBehaviour interactionBehaviour in interactable.GetInteractionBehaviours())
        {
            DetectBehaviourApplied(interactable, interactionBehaviour);
        }
    }

    //Invokes the key pressing process by checking that behaviour's specific key
    protected void DetectBehaviourApplied(Interactable interactable, IInteractionBehaviour interactionBehaviour)
    {
        if (IsInteractionKeyPressed(interactionBehaviour.InteractionKeyCode))
        {
            InteractionBehaviourEventArgs e = new(interactable, interactionBehaviour);
            OnInteractionKeyPressed?.Invoke(this, e);
        }
    }

    //5 rays are cast along the height of the player to detect more than one objects near if there are any
    protected List<Collider> GetCollidersApproached()
    {
        Vector3 rayOriginBottom = transform.position;
        Vector3 rayOriginTop = transform.position + new Vector3(0f, playerHeight, 0f);
        Vector3 direction = transform.forward; //I might have to use parent's forward vector

        List<Vector3> rayOrigins = new();

        float differenceInY = (rayOriginTop.y - rayOriginBottom.y) / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float currentY = rayOriginBottom.y + i * differenceInY;
            rayOrigins.Add(new Vector3(rayOriginBottom.x, currentY, rayOriginBottom.z));
        }

        List<Collider> hitColliders = new List<Collider>();

        foreach (Vector3 rayOrigin in rayOrigins)
        {
            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hitInfo, proximityThreshold))
            {
                if (!hitColliders.Contains(hitInfo.collider))
                {
                    hitColliders.Add(hitInfo.collider);
                }
            }
        }

        return hitColliders;
    }

    protected bool IsNear(Transform transform)
    {
        List<Collider> hitColliders = GetCollidersApproached();

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject == transform.gameObject)
                return true;
        }

        return false;
    }

    protected void RaiseInteractionConditionsMet(object sender, Interactable interactable)
    {
        OnInteractionConditionsMet?.Invoke(sender, interactable);
    }

    protected Transform GetHandTransform()
    {
        return handTransform;
    }

    protected bool AreHandsFull()
    {
        return interactableInHand != null;
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
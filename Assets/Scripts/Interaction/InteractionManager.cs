using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractionManager<T> : MonoBehaviour where T : IInteractableBehaviour
{
    public event EventHandler<List<Collider>> OnObjectCollidersApproached;
    public event EventHandler OnNoInteractableNear;
    public event EventHandler<Interactable<T>> OnInteractableApproached;
    public event EventHandler<InteractionBehaviourEventArgs> OnInteractionConditionsMet;
    public event EventHandler<InteractionBehaviourEventArgs> OnInteractionKeyPressed; //Invoked if an interactable is approached
    public event EventHandler<InteractionBehaviourEventArgs> OnInteractableInteracted; //for now, considered as the same with OnInteractionKeyPressed
    public event EventHandler<Interactable<T>> OnInteractableInHandChanged;

    public class InteractionBehaviourEventArgs : EventArgs
    {
        public Interactable<T> InteractedObject { get; }
        public T InteractionBehaviour { get; }

        public InteractionBehaviourEventArgs(Interactable<T> interactable, T interactionBehaviour)
        {
            InteractedObject = interactable;
            InteractionBehaviour = interactionBehaviour;
        }
    }

    [SerializeField] protected float proximityThreshold = 1f; //The minimum distance to an Interactable in order to detect it
    [SerializeField] protected float playerHeight = 1.67f; //Can be moved to another script
    [SerializeField] protected int rayCount = 5;
    [SerializeField] private Transform handTransform;

    protected Interactable<T> interactableInHand = null;
    private bool interactedOnceKeyIsPressed = true;

    protected virtual void Start()
    {
        OnInteractionConditionsMet += InteractionManager_InteractionConditionsMet;
        OnInteractionKeyPressed += InteractionManager_InteractionKeyPressed;
        OnInteractableInteracted += InteractionManager_InteractableInteracted;
        OnInteractableInHandChanged += InteractionManager_InteractableInHandChanged;
    }

    public Vector3 GetHandPosition()
    {
        return handTransform.position;
    }

    public Transform GetHandTransform()
    {
        return handTransform;
    }

    public void RaiseInteractableInHandChanged(Interactable<T> interactable) //Behaviours raise this event
    {
        OnInteractableInHandChanged?.Invoke(this, interactable);
    }

    protected void RaiseObjectCollidersApproached(object sender, List<Collider> hitColliders)
    {
        OnObjectCollidersApproached?.Invoke(sender, hitColliders);
    }

    protected void RaiseNoInteractableNear(object sender)
    {
        OnNoInteractableNear?.Invoke(sender, null);
    }

    protected void RaiseInteractableApproached(object sender, Interactable<T> interactable)
    {
        OnInteractableApproached?.Invoke(sender, interactable);
    }

    protected void DetectAnyColliderApproached()
    {
        List<Collider> hitColliders = GetCollidersApproached();

        switch (hitColliders.Count)
        {
            case 0:
                RaiseNoInteractableNear(this);
                break;
            default:
                RaiseObjectCollidersApproached(this, hitColliders);
                break;
        }
    }

    protected virtual void DetectInteractionConditionsMet()
    {
        Debug.Log("Base class called DetectWhenInteractionConditionsMet");
    }

    protected virtual void PlayerInteractionManager_ObjectCollidersApproached(object sender, List<Collider> colliderList)
    {
        List<Interactable<T>> interactableObjects = GetNearInteractablesList(colliderList);
        DetectInteractableApproached(interactableObjects);
    }

    protected virtual void PlayerInteractionManager_NoInteractableNear(object sender, EventArgs e)
    {
        Debug.Log("Call base class PlayerInteractionManager_NoInteractableNear");
    }

    protected virtual void PlayerInteractionManager_InteractableApproached(object sender, Interactable<T> interactable)
    {
        Debug.Log("Call base class PlayerInteractionManager_InteractableApproached");
    }

    protected virtual void InteractionManager_InteractionConditionsMet(object sender, InteractionBehaviourEventArgs e)
    {
        DetectBehaviourApplied(e.InteractedObject, e.InteractionBehaviour);
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

    protected virtual void InteractionManager_InteractableInHandChanged(object sender, Interactable<T> interactable)
    {
        interactableInHand = interactable;
    }

    protected virtual List<Interactable<T>> GetNearInteractablesList(List<Collider> colliderList)
    {
        List<Interactable<T>> interactableObjects = new();

        foreach (Collider collider in colliderList)
        {
            if (collider != null)
            {
                GameObject hitObject = collider.gameObject;

                //Making sure the object is an interactable one
                if (hitObject.TryGetComponent<Interactable<T>>(out var interactableObject))
                {
                    interactableObjects.Add(interactableObject);
                }
            }
        }

        return interactableObjects;
    }

    protected virtual void DetectInteractableApproached(List<Interactable<T>> interactableObjects)
    {
        switch (interactableObjects.Count)
        {
            case 0:
                break;
            default: //CHOOSE THE OBJECT CLOSER TO MOUSE LATER
                Interactable<T> interactable = interactableObjects[0];
                RaiseInteractableApproached(this, interactable);
                break;
        }
    }

    //Might be used when more than one behaviour can be applied to an object simultaneously
    protected void DetectBehavioursApplied(Interactable<T> interactable)
    {
        foreach (T interactionBehaviour in interactable.GetInteractionBehaviours())
        {
            DetectBehaviourApplied(interactable, interactionBehaviour);
        }
    }

    //Invokes the key pressing process by checking that behaviour's specific key
    protected void DetectBehaviourApplied(Interactable<T> interactable, T interactionBehaviour)
    {
        if (interactable.HasBehaviour(interactionBehaviour) && IsInteractionKeyPressed(interactionBehaviour.InteractionKeyCode))
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

    protected void RaiseInteractionConditionsMet(object sender, Interactable<T> interactable, T behaviour)
    {
        if (behaviour == null)
            return;
        OnInteractionConditionsMet?.Invoke(sender, new InteractionBehaviourEventArgs(interactable, behaviour));
    }

    protected bool AreHandsFull()
    {
        return interactableInHand != null;
    }

    protected void UpdateObjectInHand(Interactable<T> interactableObject)
    {
        interactableInHand = interactableObject;
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
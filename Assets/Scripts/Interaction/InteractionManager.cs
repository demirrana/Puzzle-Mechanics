using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

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


    private void Awake()
    {
        SetInstance();
    }

    private void Start()
    {
        OnObjectCollidersApproached += ObjectCollidersApproached_PlayerInteractionManager;
        OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
    }

    {
            DetectInteractableDropped();
        DetectAnObjectColliderApproached();
    }

    //5 rays are cast along the height of the player to detect more than one objects near if there are any
    protected void DetectAnObjectColliderApproached()
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

        {
            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hitInfo, proximityThreshold))
            {
                if (!hitColliders.Contains(hitInfo.collider))
                {
                    hitColliders.Add(hitInfo.collider);
                }
            }
        }

        switch (hitColliders.Count)
        {
            case 0:
                OnNoInteractableNear?.Invoke(this, null);
                break;
            default:
                OnObjectCollidersApproached?.Invoke(this, hitColliders);
                break;
        }
    }

    {
    }

    {

        foreach (Collider collider in colliderList)
        {
            if (collider != null)
            {
                GameObject hitObject = collider.gameObject;

                //Making sure the object is an interactable one
                if (hitObject.TryGetComponent<Interactable>(out var interactableObject))
                {
                    interactableObjects.Add(interactableObject);
                }
            }
        }

    }

    {
        {
        }
    }

    {
    }

    {
    }

    protected virtual bool IsInteractionKeyPressed(KeyCode interactionKeyCode)
    {
        if (Input.GetKeyDown(interactionKeyCode))
        {
            OnInteractionKeyPressed?.Invoke(this, null);
            return true;
        }

        return false;
    }
}

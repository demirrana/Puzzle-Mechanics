using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    public static PlayerInteractionManager Instance { get; private set; }

    public event EventHandler<List<Collider>> OnObjectCollidersApproached;
    public event EventHandler OnNoInteractableNear;
    public event EventHandler<Interactable> OnInteractableApproached;
    public event EventHandler OnInteractionKeyPressed; //Invoked if an interactable is approached
    public event EventHandler<Interactable> OnInteractableInteracted;

    [SerializeField] private float proximityThreshold = 1f; //The minimum distance to an Interactable in order to detect it
    [SerializeField] private float playerHeight = 1.67f; //Can be moved to another script
    [SerializeField] private int rayCount = 5;

    private void Awake()
    {
        SetInstance();
    }

    private void Start()
    {
        OnObjectCollidersApproached += ObjectCollidersApproached_PlayerInteractionManager;
        OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
    }

    private void Update()
    {
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

    protected void ObjectCollidersApproached_PlayerInteractionManager(object sender, List<Collider> colliderList)
    {
        DetectAnInteractableApproached(colliderList);
    }

    //Gets the Interactable object near and invokes the event OnInteractableApproached with it
    protected void DetectAnInteractableApproached(List<Collider> colliderList)
    {
        List<Interactable> interactableObjects = new();

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

        switch (interactableObjects.Count)
        {
            case 0:
                break;
            case 1:
                OnInteractableApproached?.Invoke(this, interactableObjects[0]);
                break;
            //TO BE CHANGED IN THE FUTURE
            default:
            Debug.Log("There are more than 1 interactables");
            //TO BE CHANGED: When there are more than 1 interactables, the camera angle should decide which one to interact with
                OnInteractableApproached?.Invoke(this, interactableObjects[1]); //For now, the one after first interactable can be interacted 
                break;
        }
        
    }

    protected void InteractableApproached_PlayerInteractionManager(object sender, Interactable interactable)
    {
        if (IsInteractionKeyPressed())
        {
            OnInteractableInteracted?.Invoke(sender, interactable);
            interactable.GetInteracted();
        }
    }

    protected virtual bool IsInteractionKeyPressed()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteractionKeyPressed?.Invoke(this, null);
            return true;
        }

        return false;
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
}

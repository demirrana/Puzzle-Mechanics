using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager5thPuzzle : InteractionManager
{
    public static InteractionManager5thPuzzle Instance { get; private set; }

    public event EventHandler<List<Collider>> OnObjectCollidersApproached;
    public event EventHandler OnNoInteractableNear;
    public event EventHandler<Interactable> OnInteractableApproached;

    private enum GameState
    {
        WorldView,
        TableView
    }

    private GameState currentState = GameState.WorldView;

    private void Awake()
    {
        SetInstance();
    }

    protected override void Start()
    {
        base.Start();
        OnObjectCollidersApproached += ObjectCollidersApproached_PlayerInteractionManager;
        OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
    }

    private void Update()
    {
        DetectInteractionConditionsMet();
    }

    protected override void DetectInteractionConditionsMet()
    {
        if (currentState == GameState.WorldView)
        {
            DetectInteractionConditionsMet_WorldView();
        }
        else
        {
            DetectInteractionConditionsMet_TableView();
        }
    }

    protected void DetectInteractionConditionsMet_WorldView()
    {
        if (AreHandsFull())
        {
            RaiseInteractionConditionsMet(this, interactableInHand); //dropping object

            if (IsNear(Interactable5thPuzzleTable.Instance.transform) && Interactable5thPuzzleTable.Instance.HasEmptySlots())
            {
                RaiseInteractionConditionsMet(this, Interactable5thPuzzleTable.Instance);
            }
        }
        else
        {
            DetectAnyColliderApproached();
        }
    }

    protected void DetectInteractionConditionsMet_TableView()
    {
        if (AreHandsFull())
        {
            //object should be dragged around
            DetectEmptySlotsOnTable();
            RaiseInteractionConditionsMet(this, interactableInHand); //exitting table view
        }
        else
        {
            DetectFullSlotsOnTable(); //this should also handle the behaviour of the object inside it (it will be dragged once obtained)
        }
        RaiseInteractionConditionsMet(this, Interactable5thPuzzleTable.Instance); //closing table view
    }

    //5 rays are cast along the height of the player to detect more than one objects near if there are any
    protected void DetectAnyColliderApproached()
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
        List<Interactable> interactableObjects = GetNearInteractablesList(colliderList);
        DetectInteractableApproached(interactableObjects);
    }

    protected virtual List<Interactable> GetNearInteractablesList(List<Collider> colliderList)
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

        return interactableObjects;
    }

    //Gets the Interactable object near and invokes the event OnInteractableApproached with it
    protected virtual void DetectInteractableApproached(List<Interactable> interactableObjects)
    {
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

    protected void RaiseInteractableApproached(object sender, Interactable interactable)
    {
        OnInteractableApproached?.Invoke(sender, interactable);
    }

    protected void InteractableApproached_PlayerInteractionManager(object sender, Interactable interactable)
    {
        foreach (IInteractionBehaviour interactionBehaviour in interactable.GetInteractionBehaviours())
        {
            DetectBehaviourApplied(interactable, interactionBehaviour);
        }
    }

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
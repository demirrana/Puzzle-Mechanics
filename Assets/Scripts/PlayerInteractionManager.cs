using System;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    public static PlayerInteractionManager Instance { get; private set; }

    public event EventHandler<Collider> OnAnObjectColliderApproached;
    public event EventHandler<Interactable> OnInteractableApproached;
    public event EventHandler OnInteractionKeyPressed; //Invoked if an interactable is approached
    public event EventHandler<Interactable> OnInteractableInteracted;

    [SerializeField] private float proximityThreshold = 1f; //The minimum distance to an Interactable in order to detect it

    private void Awake()
    {
        SetInstance();  
    }

    private void Start()
    {
        OnAnObjectColliderApproached += AnObjectColliderApproached_PlayerInteractionManager;
        OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
    }

    private void Update()
    {
        DetectAnObjectColliderApproached();
    }

    //Raycast should be detecting all the objects which are interactable and along the height of the character!!
    private void DetectAnObjectColliderApproached()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward; //I might have to use parent's forward vector
        int layerMask = LayerMask.GetMask("Interactable"); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, proximityThreshold, layerMask))
        {
            OnAnObjectColliderApproached?.Invoke(this, hitInfo.collider);
        }
    }

    private void AnObjectColliderApproached_PlayerInteractionManager(object sender, Collider collider)
    {
        DetectAnInteractableApproached(collider);
    }

    //Gets the Interactable object near and invokes the event OnInteractableApproached with it
    private void DetectAnInteractableApproached(Collider hitCollider)
    {
        //Collider hitCollider = GetAnyColliderApproached();
        if (hitCollider != null)
        {
            GameObject hitObject = hitCollider.gameObject;

             //Making sure the object is an interactable one
            Interactable interactableObject = hitObject.GetComponent<Interactable>();
            if (interactableObject != null)
            {
                //Debug.Log("Object near and its name is:" + interactableObject.name);
                OnInteractableApproached?.Invoke(this, interactableObject);
            }
        }
    }

    private void InteractableApproached_PlayerInteractionManager(object sender, Interactable interactable)
    {
        if (IsInteractionKeyPressed())
        {
            OnInteractableInteracted?.Invoke(sender, interactable);
        }
    }

    private bool IsInteractionKeyPressed()
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

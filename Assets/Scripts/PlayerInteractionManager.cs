using System;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    public static PlayerInteractionManager Instance { get; private set; }

    public event EventHandler<Collider> OnAnyObjectColliderApproached;
    public event EventHandler<Interactable> OnInteractableApproached;
    public event EventHandler OnInteractionKeyPressed;
    public event EventHandler<Interactable> OnInteractableInteracted;

    [SerializeField] private float proximityThreshold = 1f;

    private void Awake()
    {
        SetInstance();  
    }

    private void Start()
    {
        OnAnyObjectColliderApproached += AnyObjectApproached_PlayerInteractionManager;
        OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
        OnInteractionKeyPressed += InteractionKeyPressed_PlayerInteractionManager;
    }

    private void Update()
    {
        GetAnyColliderApproached();
    }

    //Raycast should be detecting all the objects which are interactable and along the height of the character!!
    private void GetAnyColliderApproached()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward; //I might have to use parent's forward vector
        int layerMask = LayerMask.GetMask("Interactable"); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, proximityThreshold, layerMask))
        {
            OnAnyObjectColliderApproached?.Invoke(this, hitInfo.collider);
        }
    }

    private void AnyObjectApproached_PlayerInteractionManager(object sender, Collider collider)
    {
        GetAnyObjectApproached(collider);
    }

    //Gets the Interactable object near and returns it
    private void GetAnyObjectApproached(Collider hitCollider)
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

    private void InteractionKeyPressed_PlayerInteractionManager(object sender, EventArgs e)
    {
        OnInteractableInteracted?.Invoke(this, null);
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

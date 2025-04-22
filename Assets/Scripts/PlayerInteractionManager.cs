using System;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    public static PlayerInteractionManager Instance { get; private set; }

    public event EventHandler OnAnyObjectApproached;
    public event EventHandler<Interactable> OnAnyObjectInteracted;

    [SerializeField] private float proximityThreshold = 1f;

    private void Awake()
    {
        SetInstance();  
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        GetAnyObjectApproached();
    }

    //Raycast should be detecting all the objects which are interactable and along the height of the character!!
    private Collider IsAnyObjectApproached()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward; //I might have to use parent's forward vector
        int layerMask = LayerMask.GetMask("Interactable"); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, proximityThreshold, layerMask))
        {
            return hitInfo.collider;
        }

        return null;
    }

    private Interactable GetAnyObjectApproached()
    {
        Collider hitCollider = IsAnyObjectApproached();
        if (hitCollider != null)
        {
            GameObject hitObject = hitCollider.gameObject;

             //Making sure the object is an interactable one
            Interactable interactableObject = hitObject.GetComponent<Interactable>();
            if (interactableObject != null)
            {
                Debug.Log("Object near and its name is:" + interactableObject.name);
                return interactableObject;
            }
        }
        
        return null;
    }

    private void Interact(Interactable interactedObject)
    {
        OnAnyObjectInteracted?.Invoke(this, interactedObject);
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

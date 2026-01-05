using UnityEngine;

public class Interactable1stPuzzleMainKey : MonoBehaviour
{
    public static Interactable1stPuzzleMainKey Instance { get; private set; }

    public float rotationSpeed = 5f;
    private Vector3 velocity;
    private bool isDragging = false;

    private Quaternion initialRotation;

    private void Awake()
    {
        SetInstance();
        InitializeVariables();
    }

    public bool IsDragging()
    {
        return isDragging;
    }

    public void RotateObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed;
            velocity = new Vector3(rotX, rotY, 0);
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 5f);
        }

        transform.Rotate(Vector3.up, -velocity.x, Space.World);
        transform.Rotate(Vector3.right, velocity.y, Space.World);
    }

    public Quaternion GetInitialRotation()
    {
        return initialRotation;
    }

    public void SetRotation(Quaternion targetRotation)
    {
        transform.rotation = targetRotation;
    }

    private void InitializeVariables()
    {
        initialRotation = transform.rotation;
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

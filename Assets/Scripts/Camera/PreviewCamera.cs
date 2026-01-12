using UnityEngine;

public class PreviewCamera : MonoBehaviour
{
    public static PreviewCamera Instance { get; private set; }

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float rotationSpeed = 30f;

    public void ResetPositionRotation()
    {
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
    }

    public void RotateAroundObject(GameObject gameObject)
    {
        transform.RotateAround(gameObject.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
    
    private void Awake()
    {
        SetInstance();
        SetVariables();
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void SetVariables()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }
}

using UnityEngine;

public class PreviewCamera : MonoBehaviour
{
    public static PreviewCamera Instance { get; private set; }

    private float rotationSpeed = 12f;

    public void RotateAroundObject(GameObject gameObject)
    {
        transform.RotateAround(gameObject.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
    
    private void Awake()
    {
        SetInstance();
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

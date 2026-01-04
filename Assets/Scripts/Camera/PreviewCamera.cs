using UnityEngine;

public class PreviewCamera : MonoBehaviour
{
    public static PreviewCamera Instance { get; private set; }
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

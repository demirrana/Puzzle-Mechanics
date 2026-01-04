using UnityEngine;

public class PreviewPanel : MonoBehaviour
{
    public static PreviewPanel Instance { get; private set; }

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

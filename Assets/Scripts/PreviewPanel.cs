using UnityEngine;

public class PreviewPanel : MonoBehaviour
{
    public static PreviewPanel Instance { get; private set; }

    private void Awake()
    {
        SetInstance();
    }

    public void Show()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
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

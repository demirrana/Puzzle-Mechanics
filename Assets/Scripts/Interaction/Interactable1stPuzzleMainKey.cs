using UnityEngine;

public class Interactable1stPuzzleMainKey : MonoBehaviour
{
    public static Interactable1stPuzzleMainKey Instance { get; private set; }

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

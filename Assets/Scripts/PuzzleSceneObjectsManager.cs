using UnityEngine;

public class PuzzleSceneObjectsManager : MonoBehaviour
{
    public static PuzzleSceneObjectsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
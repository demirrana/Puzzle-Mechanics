using UnityEngine;

public class GameplayCamera : CameraBase
{
    public static GameplayCamera Instance { get; private set; }

    private void Awake()
    {
        SetInstance();

        cameraName = CameraManager.CameraName.GameplayCamera;
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

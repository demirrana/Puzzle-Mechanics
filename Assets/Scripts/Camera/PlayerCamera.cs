using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : CameraBase
{
    public static PlayerCamera Instance { get; private set; }

    private void Awake()
    {
        SetInstance();

        cameraName = CameraManager.CameraName.PlayerCamera;
    }

    private void Start()
    {
    }

    private void Update()
    {
        
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

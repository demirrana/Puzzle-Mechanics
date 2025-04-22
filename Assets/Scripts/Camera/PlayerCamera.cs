using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : CameraBase
{
    private void Awake()
    {
        cameraName = CameraManager.CameraName.PlayerCamera;
    }

    private void Start()
    {
    }

    private void Update()
    {
        
    }

}

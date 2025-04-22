using UnityEngine;

public class PlayerDoorCamera : CameraBase
{
    private void Awake()
    {
        cameraName = CameraManager.CameraName.PlayerDoorCamera;
    }
}

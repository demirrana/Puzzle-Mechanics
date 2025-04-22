using UnityEngine;

public class KidCamera : CameraBase
{
    private void Awake()
    {
        cameraName = CameraManager.CameraName.KidCamera;
    }
}

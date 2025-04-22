using UnityEngine;

public class NeighborDoorCamera : CameraBase
{
    private void Awake()
    {
        cameraName = CameraManager.CameraName.NeighborDoorCamera;
    }
}

using System;
using UnityEngine;

public class CameraBase : MonoBehaviour
{
    protected CameraManager.CameraName cameraName;
    protected event EventHandler OnCameraActivated; //Might take the enum value as parameter.

    public CameraManager.CameraName GetCameraName()
    {
        return cameraName;
    }
}

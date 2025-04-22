using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public CameraManager Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCameraBase[] cameraList;

    public enum CameraName 
    {
        PlayerCamera,
        KidCamera,
        OldManCamera,
        PlayerDoorCamera,
        NeighborDoorCamera,

    }

    private CameraName activeCameraName;

    private void Awake()
    {
        SetInstance();

        activeCameraName = CameraName.PlayerCamera; //Can be changed based on the game's first look
        SetActiveCamera(activeCameraName);
    }

    void Update()
    {
        //To be deleted
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cameraList[0].Priority > cameraList[1].Priority)
            {
                cameraList[1].Priority = 10;
                cameraList[0].Priority = 0;
            }
            else
            {
                cameraList[0].Priority = 10;
                cameraList[1].Priority = 0;
            }
        }
    }

    //Make the switches event based. Hold an event for our character speaking. Then hold one for the child speaking etc.

    //Switches to the new camera and adjusts priorities in order to do that.
    private void SetActiveCamera(CameraName cameraName)
    {
        GetCameraUnderTheName(cameraName).Priority = 10;
        GetCameraUnderTheName(activeCameraName).Priority = 0;
        
        activeCameraName = cameraName;
    }

    public CinemachineVirtualCameraBase GetCameraUnderTheName(CameraName cameraName)
    {
        foreach (CinemachineVirtualCameraBase virtualCamera in cameraList)
        {
            if (cameraName == virtualCamera.GetComponent<CameraBase>().GetCameraName())
            {
                return virtualCamera;
            }
        }

        return null;
    }

    public CameraName GetActiveCameraName()
    {
        return activeCameraName;
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }
}

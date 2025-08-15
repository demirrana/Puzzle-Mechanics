using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private List<CinemachineVirtualCameraBase> cameraList;

    public enum CameraName 
    {
        PlayerCamera,
        GameplayCamera,
    }

    private CameraName activeCameraName;
    private int activeCameraIndex;
    private CameraBase activeCamera;
    private Dictionary<CameraName, CameraBase> cameraMap; //Will be changed to a List of KeyValuePair.

    private void Awake()
    {
        SetInstance();

        InitializeCameraMap();

        activeCameraName = CameraName.PlayerCamera; //Can be changed based on the game's first look
        activeCameraIndex = 0;
        activeCamera = PlayerCamera.Instance;
        SetActiveCamera(activeCameraName);
    }

    private void Update()
    {
        //To be deleted
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchToNextCamera();
        }
    }

    //Make the switches event based. Hold an event for our character speaking. Then hold one for the child speaking etc.

    //Switches to the new camera and adjusts priorities in order to do that.
    private void SetActiveCamera(CameraName cameraName)
    {
        GetCameraUnderTheName(cameraName).Priority = 10;
        GetCameraUnderTheName(activeCameraName).Priority = 0;

        //activeCameraIndex = cameraMap. 
        activeCameraName = cameraName;
        //Debug.Log("active camera name: " + activeCameraName);
        //Debug.Log("cameraMap 0th element: " + cameraMap.ElementAt(0).ToString());
        //activeCamera = cameraMap.ElementAt(activeCameraIndex).Value;
    }

    public void SwitchToNextCamera()
    {
        if (activeCameraIndex == cameraMap.Count - 1)
        {
            activeCameraIndex = 0;
        }
        else
            activeCameraIndex++;

        SetActiveCamera(cameraMap.Keys.ElementAt(activeCameraIndex));
    }

    public CinemachineVirtualCameraBase GetCameraUnderTheName(CameraName cameraName)
    {
        foreach (CinemachineVirtualCameraBase virtualCamera in cameraList)
        {
            if (cameraName == virtualCamera.GetComponent<CameraBase>().GetCameraName())
            {
                //Debug.Log("Camera found and not null");
                return virtualCamera;
            }
        }

        return null;
    }

    public CameraName GetActiveCameraName()
    {
        return activeCameraName;
    }

    public CameraBase GetActiveCameraBase() //might be CinemachineVirtualCameraBase instead of CameraBase
    {
        return activeCamera;
    }

    private void InitializeCameraMap()
    {
        cameraMap = new Dictionary<CameraName, CameraBase>
        {
            { CameraName.PlayerCamera, PlayerCamera.Instance },
            { CameraName.GameplayCamera, GameplayCamera.Instance },
        };
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

using UnityEngine;

public class InputActionsManager : MonoBehaviour
{
    public static InputActionsManager Instance { get; private set; }

    public PlayerInputActions PlayerInputActions { get; private set; }

    private void Awake()
    {
        //Below ones should execute before PlayerCamera's and PlayerMovementManager's.
        SetInstance();

        PlayerInputActions = new PlayerInputActions();
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

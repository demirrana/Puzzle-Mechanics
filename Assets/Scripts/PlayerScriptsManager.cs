using UnityEngine;

public class PlayerScriptsManager : MonoBehaviour
{
    public static PlayerScriptsManager Instance { get; private set; }

    private PlayerMovementManager playerMovementManager;
    private InteractionManager playerInteractionManager;

    private void Awake()
    {
        SetInstance();
    }

    private void Start()
    {
        playerMovementManager = PlayerMovementManager.Instance;
        playerInteractionManager = InteractionManager.Instance;
    }

    public bool IsMovementEnabled()
    {
        return playerMovementManager.enabled;
    }

    public bool IsInteractionEnabled()
    {
        return playerInteractionManager.enabled;
    }

    public void DisableAllScripts()
    {
        playerMovementManager.enabled = false;
        playerInteractionManager.enabled = false;
    }

    public void DisableMovementScript()
    {
        playerMovementManager.enabled = false;
    }

    public void DisableInteractionScript()
    {
        playerInteractionManager.enabled = false;
    }

    public void EnableAllScripts()
    {
        playerMovementManager.enabled = true;
        playerInteractionManager.enabled = true;
    }

    public void EnableMovementScript()
    {
        playerMovementManager.enabled = true;
    }

    public void EnableInteractionScript()
    {
        playerInteractionManager.enabled = true;
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

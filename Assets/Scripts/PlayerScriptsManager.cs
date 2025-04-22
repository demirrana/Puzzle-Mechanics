using UnityEngine;

public class PlayerScriptsManager : MonoBehaviour
{
    private PlayerMovementManager playerMovementManager;
    private PlayerInteractionManager playerInteractionManager;

    private void Start()
    {
        playerMovementManager = PlayerMovementManager.Instance;
        playerInteractionManager = PlayerInteractionManager.Instance;
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
}

using UnityEngine;

public class PlayerScriptsManager : MonoBehaviour
{
    public void DisableAllScripts()
    {
        PlayerMovementManager.Instance.enabled = false;
    }
    public void DisableMovementScript()
    {
        PlayerMovementManager.Instance.enabled = false;
    }

    public void EnableAllScripts()
    {
        PlayerMovementManager.Instance.enabled = true;
    }

    public void EnableMovementScript()
    {
        PlayerMovementManager.Instance.enabled = true;
    }
}

using UnityEngine;

public class PlayerScriptsManager : MonoBehaviour
{
    public static PlayerScriptsManager Instance { get; private set; }

    private PlayerMovementManager playerMovementManager;
    private InteractionManager5thPuzzle interactionManager5thPuzzle;

    private void Awake()
    {
        SetInstance();
    }

    private void Start()
    {
        playerMovementManager = PlayerMovementManager.Instance;
        interactionManager5thPuzzle = InteractionManager5thPuzzle.Instance;
    }

    public bool IsMovementEnabled()
    {
        return playerMovementManager.enabled;
    }

    public bool IsInteractionEnabled()
    {
        return interactionManager5thPuzzle.enabled;
    }

    public void DisableAllScripts()
    {
        playerMovementManager.enabled = false;
        interactionManager5thPuzzle.enabled = false;
    }

    public void DisableMovementScript()
    {
        playerMovementManager.enabled = false;
    }

    public void DisableInteractionScript()
    {
        interactionManager5thPuzzle.enabled = false;
    }

    public void EnableAllScripts()
    {
        playerMovementManager.enabled = true;
        interactionManager5thPuzzle.enabled = true;
    }

    public void EnableMovementScript()
    {
        playerMovementManager.enabled = true;
    }

    public void EnableInteractionScript()
    {
        interactionManager5thPuzzle.enabled = true;
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

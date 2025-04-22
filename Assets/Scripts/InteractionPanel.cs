using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPanel : MonoBehaviour
{
    public static InteractionPanel Instance { get; private set; }

    [SerializeField] private Image interactionPanelBackground;
    [SerializeField] private TextMeshProUGUI interactionKeyText;

    private void Awake()
    {
        SetInstance();
        Hide();
    }

    private void Start()
    {
        PlayerInteractionManager.Instance.OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
        PlayerInteractionManager.Instance.OnInteractableInteracted += InteractableInteracted_PlayerInteractionManager;
    }

    private void InteractableApproached_PlayerInteractionManager(object sender, Interactable interactable)
    {
        Show();
    }

    private void InteractableInteracted_PlayerInteractionManager(object sender, Interactable interactable)
    {
        Hide();
    }

    private void Hide()
    {
        interactionPanelBackground.enabled = false;
        interactionKeyText.enabled = false;
    }

    private void Show()
    {
        interactionPanelBackground.enabled = true;
        interactionKeyText.enabled = true;
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

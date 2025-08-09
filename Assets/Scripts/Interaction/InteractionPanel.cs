using System;
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
        PlayerInteractionManager.Instance.OnNoInteractableNear += NoInteractableNear_PlayerInteractionManager;
    }

    private void InteractableApproached_PlayerInteractionManager(object sender, Interactable interactable)
    {
        //Debug.Log("Interactable approached");
        Show();
    }

    private void InteractableInteracted_PlayerInteractionManager(object sender, PlayerInteractionManager.InteractionBehaviourEventArgs e)
    {
        //Debug.Log("Interactable interacted");
        Hide();
    }

    private void NoInteractableNear_PlayerInteractionManager(object sender, EventArgs e)
    {
        //Debug.Log("No Interactable approached");
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

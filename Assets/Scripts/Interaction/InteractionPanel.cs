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
        InteractionManager5thPuzzle.Instance.OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
        InteractionManager5thPuzzle.Instance.OnInteractableInteracted += InteractableInteracted_PlayerInteractionManager;
        InteractionManager5thPuzzle.Instance.OnNoInteractableNear += NoInteractableNear_PlayerInteractionManager;
    }

    //Interactable<T> may be required for more manager classes handling
    private void InteractableApproached_PlayerInteractionManager(object sender, Interactable<IInteractableBehaviour5thPuzzle> interactable)
    {
        //Debug.Log("Interactable approached");
        Show();
    }

    //Interactable<T> may be required for more manager classes handling
    private void InteractableInteracted_PlayerInteractionManager(object sender, InteractionManager<IInteractableBehaviour5thPuzzle>.InteractionBehaviourEventArgs e)
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

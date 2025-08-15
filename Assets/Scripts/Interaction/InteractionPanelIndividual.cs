using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPanelIndividual : MonoBehaviour
{
    public static InteractionPanelIndividual Instance { get; private set; }

    public event EventHandler<Transform> OnInteractionPanelActivated;
    public event EventHandler OnInteractionPanelDeactivated;

    [SerializeField] private RectTransform uiElement;
    [SerializeField] private Image interactionPanelBackground;
    [SerializeField] private TextMeshProUGUI interactionKeyText;

    private void Awake()
    {
        SetInstance();
        Hide();
    }

    private void Start()
    {
        OnInteractionPanelActivated += InteractionPanelActivated;
        OnInteractionPanelDeactivated += InteractionPanelDeactivated;
    }

    private void Update()
    {
        if (IsDisplayed())
        {
            //uiElement.rotation = Camera.main.transform.rotation;
        }
    }

    public void RaiseInteractionPanelActivated(object sender, Transform targetTransform)
    {
        OnInteractionPanelActivated?.Invoke(sender, targetTransform);
    }

    public void RaiseInteractionPanelDeactivated(object sender)
    {
        OnInteractionPanelDeactivated?.Invoke(sender, null);
    }

    private void InteractionPanelActivated(object sender, Transform targetTransform)
    {
        UpdatePosition(targetTransform);
        Show();
    }

    private void InteractionPanelDeactivated(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdatePosition(Transform targetTransform)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetTransform.position);
        uiElement.position = screenPos;
        //uiElement.position = targetTransform.position + Vector3.up;
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

    private bool IsDisplayed()
    {
        return interactionPanelBackground.enabled;
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
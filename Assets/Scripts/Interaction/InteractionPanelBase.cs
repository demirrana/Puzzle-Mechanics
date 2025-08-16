using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPanelBase : MonoBehaviour
{
    [SerializeField] protected RectTransform uiElement;
    [SerializeField] protected Image interactionPanelBackground;
    [SerializeField] protected TextMeshProUGUI interactionKeyText;

    protected virtual void Awake()
    {
        Hide();
    }

    protected virtual void UpdateInteractionKeyText(String newText)
    {
        interactionKeyText.text = newText;
    }

    protected void Hide()
    {
        interactionPanelBackground.enabled = false;
        interactionKeyText.enabled = false;
    }

    protected void Show()
    {
        interactionPanelBackground.enabled = true;
        interactionKeyText.enabled = true;
    }
}
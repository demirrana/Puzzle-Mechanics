using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPanelIndividual : InteractionPanelBase
{
    public static InteractionPanelIndividual Instance { get; private set; }

    public class TransformKeyCodeEventArgs : EventArgs
    {
        public Transform PanelTargetTransform;
        public KeyCode InteractionKeyCode;

        public TransformKeyCodeEventArgs(Transform transform, KeyCode keyCode)
        {
            PanelTargetTransform = transform;
            InteractionKeyCode = keyCode;
        }
    }

    public event EventHandler<TransformKeyCodeEventArgs> OnInteractionPanelActivated;
    public event EventHandler OnInteractionPanelDeactivated;

    //[SerializeField] private RectTransform uiElement;
    //[SerializeField] private Image interactionPanelBackground;
    //[SerializeField] private TextMeshProUGUI interactionKeyText;

    protected override void Awake()
    {
        SetInstance();
        base.Awake();
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

    public void RaiseInteractionPanelActivated(object sender, Transform targetTransform, KeyCode behaviourKeyCode)
    {
        OnInteractionPanelActivated?.Invoke(sender, new TransformKeyCodeEventArgs(targetTransform, behaviourKeyCode));
    }

    public void RaiseInteractionPanelDeactivated(object sender)
    {
        OnInteractionPanelDeactivated?.Invoke(sender, null);
    }

    private void InteractionPanelActivated(object sender, TransformKeyCodeEventArgs e)
    {
        UpdateInteractionKeyText(e.InteractionKeyCode.ToString());
        UpdatePosition(e.PanelTargetTransform);
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
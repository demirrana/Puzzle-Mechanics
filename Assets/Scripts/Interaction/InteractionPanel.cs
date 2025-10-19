using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPanel : InteractionPanelBase
{
    public static InteractionPanel Instance { get; private set; }

    //[SerializeField] private Image interactionPanelBackground;
    //[SerializeField] private TextMeshProUGUI interactionKeyText;

    protected override void Awake()
    {
        SetInstance();
        base.Awake();
    }

    private void Start()
    {
        InteractionManager0thPuzzle.Instance.OnInteractionConditionsMet += InteractionConditionsMet_InteractionManager0thPuzzle;
        InteractionManager5thPuzzle.Instance.OnInteractionConditionsMet += InteractionConditionsMet_InteractionManager5thPuzzle;
        //InteractionManager5thPuzzle.Instance.OnInteractableApproached += InteractableApproached_PlayerInteractionManager;
        InteractionManager5thPuzzle.Instance.OnInteractableInteracted += InteractableInteracted_PlayerInteractionManager;
        InteractionManager5thPuzzle.Instance.OnNoInteractableNear += NoInteractableNear_PlayerInteractionManager;
        InteractionManager7thPuzzle.Instance.OnBothInteractablesAreChosen += BothInteractablesAreChosen_InteractionPanel;
        InteractionManager7thPuzzle.Instance.OnAnyInteractableIsDeselected += AnyInteractableIsDeselected_InteractionPanel;
    }

    private void InteractionConditionsMet_InteractionManager0thPuzzle(object sender, InteractionManager0thPuzzle.InteractionBehaviourEventArgs e)
    {
        //Handle displaying the key for that behaviour
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

    private void BothInteractablesAreChosen_InteractionPanel(object sender, KeyCode swapKey)
    {
        UpdateInteractionKeyText(swapKey.ToString());
        Show();
    }

    private void AnyInteractableIsDeselected_InteractionPanel(object sender, EventArgs e)
    {
        Hide();
    }

    private void InteractionConditionsMet_InteractionManager5thPuzzle(object sender, InteractionManager5thPuzzle.InteractionBehaviourEventArgs e)
    {
        bool isDragOnTableFromHand = e.InteractionBehaviour.GetType() == typeof(InteractableBehaviourDragOnTableFromHand);
        bool isDropOnFloor = e.InteractionBehaviour.GetType() == typeof(InteractableBehaviourDropOnFloor);
        bool isPutOnTableSlot = e.InteractionBehaviour.GetType() == typeof(InteractableBehaviourPutOnTableSlot);
        bool isPickUpFromSlot = e.InteractionBehaviour.GetType() == typeof(InteractableBehaviourDragOnTableFromSlot);
        bool isPickUpFromTableToHand = e.InteractionBehaviour.GetType() == typeof(InteractableBehaviourPickUpFromTableToHand);

        if (isDragOnTableFromHand || isDropOnFloor || isPutOnTableSlot || isPickUpFromSlot || isPickUpFromTableToHand)
        {
            Show();
        }
        else
        {
            Hide();
        }
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

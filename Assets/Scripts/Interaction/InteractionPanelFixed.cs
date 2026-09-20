using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPanelFixed : InteractionPanelBase
{
    public static InteractionPanelFixed Instance { get; private set; }

    [SerializeField] private Color originalBackgroundColor;
    private readonly Color swapKeyPressColor = new(0.3f, 0.35f, 0f, 0.4f);

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
        Interactable5thPuzzleTable.Instance.OnTableViewActivated += Puzzle5Table_TableViewActivated;
        Interactable5thPuzzleTable.Instance.OnTableViewDeactivated += Puzzle5Table_TableViewDeactivated;
        InteractionManager5thPuzzle.Instance.OnInteractableInteracted += InteractableInteracted_InteractionManager5thPuzzle;
        InteractionManager7thPuzzle.Instance.OnBothInteractablesAreChosen += BothInteractablesAreChosen_InteractionPanel;
        InteractionManager7thPuzzle.Instance.OnAnyInteractableIsDeselected += AnyInteractableIsDeselected_InteractionPanel;
        InteractionManager7thPuzzle.Instance.OnSwapKeyPressed += SwapKeyPressed_InteractionPanel;
    }

    private void OnDestroy()
    {
        InteractionManager0thPuzzle.Instance.OnInteractionConditionsMet -= InteractionConditionsMet_InteractionManager0thPuzzle;
        InteractionManager5thPuzzle.Instance.OnInteractionConditionsMet -= InteractionConditionsMet_InteractionManager5thPuzzle;
        Interactable5thPuzzleTable.Instance.OnTableViewActivated -= Puzzle5Table_TableViewActivated;
        Interactable5thPuzzleTable.Instance.OnTableViewDeactivated -= Puzzle5Table_TableViewDeactivated;
        InteractionManager5thPuzzle.Instance.OnInteractableInteracted -= InteractableInteracted_InteractionManager5thPuzzle;
        InteractionManager7thPuzzle.Instance.OnBothInteractablesAreChosen -= BothInteractablesAreChosen_InteractionPanel;
        InteractionManager7thPuzzle.Instance.OnAnyInteractableIsDeselected -= AnyInteractableIsDeselected_InteractionPanel;
        InteractionManager7thPuzzle.Instance.OnSwapKeyPressed -= SwapKeyPressed_InteractionPanel;
    }

    public void UpdateBackgroundColor() //go back to original background color
    {
        interactionPanelBackground.color = originalBackgroundColor;
    }

    public void UpdateBackgroundColor(Color newColor)
    {
        interactionPanelBackground.color = newColor;
    }

    private void InteractionConditionsMet_InteractionManager0thPuzzle(object sender, InteractionManager0thPuzzle.InteractionBehaviourEventArgs e)
    {
        //Handle displaying the key for that behaviour
        UpdateInteractionKeyText(e.InteractionBehaviour.InteractionKeyCode.ToString());
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

    private void SwapKeyPressed_InteractionPanel(object sender, EventArgs e)
    {
        StartCoroutine(DisplaySwapKeyPressAndHide());
    }

    private IEnumerator DisplaySwapKeyPressAndHide() //display the swap key press color for a short duration and then hide the panel
    {
        UpdateBackgroundColor(swapKeyPressColor);
        yield return new WaitForSeconds(0.25f);
        Hide();
        UpdateBackgroundColor();
    }

    private void InteractionConditionsMet_InteractionManager5thPuzzle(object sender, InteractionManager5thPuzzle.InteractionBehaviourEventArgs e)
    {
        if (!InteractionManager5thPuzzle.Instance.AreHandsFull()) return; //this event gets triggered before dropping obj, so check it

        bool isDropOnFloor = e.InteractionBehaviour.GetType() == typeof(InteractableBehaviourDropOnFloor);
        if (isDropOnFloor)
        {
            UpdateInteractionKeyText(e.InteractionBehaviour.InteractionKeyCode.ToString());
            Show();
        }
    }

    private void Puzzle5Table_TableViewActivated(object sender, EventArgs e)
    {
        UpdateInteractionKeyText(Interactable5thPuzzleTable.Instance.GetKeyForCloseTableView().ToString());
        Show();
    }

    //Hide "drop object key" after it is dropped
    private void InteractableInteracted_InteractionManager5thPuzzle(object sender, InteractionManager5thPuzzle.InteractionBehaviourEventArgs e)
    {
        bool isDropOnFloor = e.InteractionBehaviour.GetType() == typeof(InteractableBehaviourDropOnFloor);

        if (isDropOnFloor)
        {
            Hide();
        }
    }

    private void Puzzle5Table_TableViewDeactivated(object sender, EventArgs e)
    {
        Hide();
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

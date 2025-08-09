using System;
using UnityEngine;

public class Interactable5thPuzzle : Interactable
{
    public event EventHandler<Interactable5thPuzzleState> OnInteractableStateChanged; //Might as well be in the Interactable
    public event EventHandler OnInteractableInHand;

    public enum Interactable5thPuzzleState
    {
        InHand,
        OnFloor,
        OnTable,
        LoadingTableView,
        ExittingTableView
    }

    private Interactable5thPuzzleState currentState;

    private void Awake()
    {
        currentState = Interactable5thPuzzleState.OnFloor;
        behavioursList.Add(new InteractableBehaviourCollectFromFloor());
    }

    protected override void Start()
    {
        //Debug.Log($"[Child Start] {name} subscribing to OnInteractableStateChanged. InstanceID={GetInstanceID()}");
        base.Start();
        OnInteractableStateChanged += Interactable5thPuzzle_OnInteractableStateChanged;
    }

    protected override void GetInteracted_Interactable(object sender, IInteractionBehaviour interactionBehaviour)
    {
        //Debug.Log($"[Child Handler] invoked on {name}, target type: {GetType().Name} (InstanceID {GetInstanceID()})");
        Debug.Log("GetInteracted_Interactable of Interactable5thPuzzle is called.");
        interactionBehaviour.Interact(this);
        Debug.Log("Behaviour keycode: " + interactionBehaviour.InteractionKeyCode.ToString());
    }

    public void UpdateState(Interactable5thPuzzleState newState)
    {
        currentState = newState;
        Debug.Log("State is updated to " + currentState.ToString());
        OnInteractableStateChanged?.Invoke(this, newState);
    }

    override public void UpdatePosition(Transform newTransform)
    {
        gameObject.transform.position = newTransform.position;
    }

    private void Interactable5thPuzzle_OnInteractableStateChanged(object sender, Interactable5thPuzzleState newState)
    {
        Debug.Log("StateChanged event is triggered.");
        switch (newState)
        {
            case Interactable5thPuzzleState.InHand:
                UpdatePosition(PlayerInteractionManager.Instance.transform);
                gameObject.transform.SetParent(PlayerInteractionManager.Instance.transform);
                break;
            case Interactable5thPuzzleState.OnFloor:
                break;
            case Interactable5thPuzzleState.OnTable:
                break;
            default:
                break;
        }
    }
}
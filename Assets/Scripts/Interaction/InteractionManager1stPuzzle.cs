using UnityEngine;

public class InteractionManager1stPuzzle : InteractionManager<IInteractableBehaviour1stPuzzle> //to be changed into 1stPuzzle
{
    public static InteractionManager1stPuzzle Instance { get; private set; }


    public enum ViewMode
    {
        WorldView,
        EditView
    }


    private InteractionPanelIndividual InteractionPanelIndividual;
    private ViewMode currentViewMode;


    private void Awake()
    {
        SetInstance();
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
    }

    private void InitializeVariables()
    {
        InteractionPanelIndividual = InteractionPanelIndividual.Instance;
    }

    {
        {
        }
    }
}
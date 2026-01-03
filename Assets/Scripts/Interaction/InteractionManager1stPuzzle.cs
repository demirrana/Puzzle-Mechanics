using UnityEngine;

public class InteractionManager1stPuzzle : InteractionManager<IInteractableBehaviour1stPuzzle> //to be changed into 1stPuzzle
{
    public static InteractionManager1stPuzzle Instance { get; private set; }


    private InteractionPanelIndividual InteractionPanelIndividual;

    private void Awake()
    {
        SetInstance();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void InitializeInstances()
    {
        InteractionPanelIndividual = InteractionPanelIndividual.Instance;
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
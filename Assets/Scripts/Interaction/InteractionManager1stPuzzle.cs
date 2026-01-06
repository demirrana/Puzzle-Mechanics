using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractionManager1stPuzzle : InteractionManager<IInteractableBehaviour1stPuzzle> //to be changed into 1stPuzzle
{
    public static InteractionManager1stPuzzle Instance { get; private set; }

    public event EventHandler<Interactable1stPuzzleObject> OnAnyKeyPartCollected;

    public enum ViewMode
    {
        WorldView,
        EditView
    }

    [SerializeField] private Interactable<IInteractableBehaviour1stPuzzle> mainKeyPartPrefab;

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
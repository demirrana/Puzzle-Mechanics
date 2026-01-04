using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interactable1stPuzzleObject : Interactable1stPuzzle
{
    [SerializeField] private SOCollectibleKeyPart keyPartData;

    private void Awake()
    {
        behavioursList.Add(new InteractableBehaviourCollectKeyPart());
    }

    public void SetParent(Transform parentTransform)
    {
        transform.parent = parentTransform;
    }

    public SOCollectibleKeyPart GetKeyPartData()
    {
        return keyPartData;
    }

    public String GetKeyPartID()
    {
        return keyPartData.keyPartID;
    }

    public String GetDisplayName()
    {
        return keyPartData.displayName;
    }

    public Sprite GetInventoryIcon()
    {
        return keyPartData.inventoryIcon;
    }

    public GameObject GetPrefab()
    {
        return keyPartData.prefab;
    }
}

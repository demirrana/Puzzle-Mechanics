using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interactable1stPuzzleObject : Interactable1stPuzzle
{
    public List<SocketData> sockets;

    [SerializeField] private SOCollectibleKeyPart keyPartData;

    [SerializeField] private Transform plugTransform;
    [SerializeField] private String plugID;

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

    public String GetPlugID()
    {
        return plugID;
    }

    public Transform GetPlugTransform()
    {
        return plugTransform;
    }
}

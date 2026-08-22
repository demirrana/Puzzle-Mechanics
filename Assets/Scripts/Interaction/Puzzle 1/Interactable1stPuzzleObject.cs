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

    private SocketData currentSocket;

    private void Awake()
    {
        behavioursList.Add(new InteractableBehaviourCollectKeyPart());
        currentSocket = null;
    }

    public void SetParent(Transform parentTransform)
    {
        transform.parent = parentTransform;
    }

    public void Snap(SocketData socket)
    {
        currentSocket = socket;
    }

    public void Unsnap()
    {
        currentSocket = null;
    }

    public SocketData GetHostSocket()
    {
        return currentSocket;
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

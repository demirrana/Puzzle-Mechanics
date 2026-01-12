using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SocketData
{
    public List<String> targetPlugIDs;
    public Transform socketTransform;
    public bool isOccupied;
    public Quaternion targetRotation;
    public Interactable1stPuzzleObject snappedKey;

    public void Empty()
    {
        isOccupied = true;
        snappedKey = null;
    }

    public void SnapKey(Interactable1stPuzzleObject key)
    {
        isOccupied = false;
        snappedKey = key;
    }
}
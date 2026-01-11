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
}
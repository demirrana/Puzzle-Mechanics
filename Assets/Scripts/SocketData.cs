using System;
using UnityEngine;

[Serializable]
public class SocketData
{
    public String targetPlugID;
    public Transform socketTransform;
    public bool isOccupied;
    public Quaternion targetRotation;
}
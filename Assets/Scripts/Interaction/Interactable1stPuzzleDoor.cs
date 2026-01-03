using System;
using UnityEngine;

public class Interactable1stPuzzleDoor : MonoBehaviour
{
    private bool hasOpened;
    private static KeyCode interactionKeyCode = KeyCode.Alpha2;

    private void Awake()
    {
        InitializeInstances();
    }

    public KeyCode GetInteractionKey()
    {
        return interactionKeyCode;
    }

    private void InitializeInstances()
    {
        hasOpened = false;
    }
}

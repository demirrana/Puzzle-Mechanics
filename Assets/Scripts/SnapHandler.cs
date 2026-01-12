using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class SnapHandler : MonoBehaviour
{
    public static SnapHandler Instance { get; private set; }

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
}

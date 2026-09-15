using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Puzzle1stObject : MonoBehaviour
{
    [SerializeField] private Image keyPartIcon;
    [SerializeField] private TMP_Text keyPartName;

    private SOCollectibleKeyPart keyPartData;
    private Interactable1stPuzzleObject keyPart;

    public void Setup(SOCollectibleKeyPart keyPartData, Interactable1stPuzzleObject keyPart)
    {
        keyPartIcon.sprite = keyPartData.inventoryIcon;
        keyPartName.text = keyPartData.displayName;
        this.keyPartData = keyPartData;
        this.keyPart = keyPart;
    }

    public GameObject GetKeyPartObject()
    {
        return keyPartData.prefab;
    }

    public SOCollectibleKeyPart GetKeyPartData()
    {
        return keyPartData;
    }

    public Interactable1stPuzzleObject GetKeyPart()
    {
        return keyPart;
    }

    public void Hide()
    {
        gameObject.SetActive(false); //will be changed to child's activeness because parent will be empty and mesh will be the child
    }

    public void Show()
    {
        gameObject.SetActive(true); //will be changed to child's activeness because parent will be empty and mesh will be the child
    }
}

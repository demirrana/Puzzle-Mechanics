using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Puzzle1stObject : MonoBehaviour
{
    [SerializeField] private Image keyPartIcon;
    [SerializeField] private TMP_Text keyPartName;

    private SOCollectibleKeyPart keyPartData;
    private Interactable1stPuzzleObject keyPart;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}

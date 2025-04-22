using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialoguePanel : MonoBehaviour
{
    public static DialoguePanel Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBackgroundImage;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Image dayBackgroundImage;

    private void Awake()
    {
        SetInstance();
        HideDialoguePanel();
        HideDayPanel();
    }

    private void Start()
    {
        DialogueManager.Instance.OnDayTextActivated += ManageOnDayPanelActivated;
        DialogueManager.Instance.OnDialogueActivated += ManageOnDialoguePanelActivated;
        DialogueManager.Instance.OnDialogueDeactivated += ManageOnDialoguePanelDeactivated;
    }

    public void SetDialogue(String dialogueInput)
    {
        dialogueText.text = dialogueInput;
    }

    public void SetDayText(String dayTextInput)
    {
        dayText.text = dayTextInput;
    }

    public bool IsDialogueActive()
    {
        return dialogueText.enabled;
    }

    public bool IsDayTextActive()
    {
        return dayText.enabled;
    }

    public void DeactivatePanel() //Hides the active text space (dialogue or day text)
    {
        if (IsDialogueActive())
        {
            HideDialoguePanel();
        }
        else if (IsDayTextActive())
        {
            HideDayPanel();
        }
    }

    public void ChangeDialoguePanelVisibility()
    {
        if (IsDialogueActive())
        {
            HideDialoguePanel();
        }
        else
        {
            ShowDialoguePanel();
        }
    }

    public void ChangeDayPanelVisibility()
    {
        if (IsDialogueActive())
        {
            HideDayPanel();
        }
        else
        {
            ShowDayPanel();
        }
    }

    private void HideDialoguePanel()
    {
        dialogueText.enabled = false;
        dialogueBackgroundImage.enabled = false;
    }

    private void ShowDialoguePanel()
    {
        dialogueText.enabled = true;
        dialogueBackgroundImage.enabled = true;
    }

    private void HideDayPanel()
    {
        dayText.enabled = false;
        dayBackgroundImage.enabled = false;
    }

    private void ShowDayPanel()
    {
        dayText.enabled = true;
        dayBackgroundImage.enabled = true;
    }

    private void ManageOnDayPanelActivated(object sender, EventArgs e)
    {
        ShowDayPanel();

        StartCoroutine(WaitForDayPanelToBeClosed());
    }

    private void ManageOnDialoguePanelActivated(object sender, EventArgs e)
    {
        ShowDialoguePanel();
    }

    private void ManageOnDialoguePanelDeactivated(object sender, EventArgs e)
    {
        StartCoroutine(WaitForDialoguePanelToBeClosed());
    }

    private IEnumerator WaitForDayPanelToBeClosed()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        HideDayPanel();
    }

    private IEnumerator WaitForDialoguePanelToBeClosed()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        HideDialoguePanel();
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

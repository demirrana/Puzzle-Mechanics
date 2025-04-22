using System;
using UnityEngine;
using System.IO;
using System.Collections;
using UnityEditor.Search;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public event EventHandler OnDialogueActivated;
    public event EventHandler OnDialogueDeactivated;

    public event EventHandler OnDayTextActivated;

    public const string jsonFileName = "dialogues"; //name can be changed since it's constant

    private DialoguePanel dialoguePanel; //Not used, use it instead of DialoguePanel.Instance

    private DialogueRoot dialogueRoot;

    private bool isTextModeOn = false; //All texts are displayed while it is true

    private int dayIndex = 0;
    private int dialogueIndex = 0;
    private bool isDayDisplayed = false; //Day text is shown for that day when it is true and stays false until the next day comes
    private bool willNextLineBeShown = true;

    private void Awake()
    {
        SetInstance();
    }

    private void Start()
    {
        LoadDialogue();
        dialoguePanel = DialoguePanel.Instance; //This should work after DialoguePanel's Awake method
    }

    private bool displayed = false; //is only used by the method LogDialogues and it is currently not used.

    private void Update()
    {
        ManageDialogueModeActiveness();
        DisplayTexts();
    }

    private void ManageDialogueModeActiveness()
    {
        if (Input.GetKeyDown(KeyCode.T)) //This condition will be changed with the one to activate/deactivite dialogues or will be moved into another class
        {
            //Debug.Log("T pressed");
            isTextModeOn = true;
            willNextLineBeShown = true;
        }
    }

    public void DisplayTexts()
    {
        if (isTextModeOn)
            ManageDisplayingNextLine();
    }

    private void ManageDisplayingNextLine()
    {
        if (willNextLineBeShown)
        {
            if (!isDayDisplayed) //Day text is displayed
                DisplayNextLine();
            else if (isDayDisplayed && Input.GetKeyDown(KeyCode.Return)) //Dialogues after first one require "Return" key to be displayed
            {
                Debug.Log("Dialogue requiring Return");
                DisplayNextLine();
            }
            else if (isDayDisplayed && dialogueIndex == 0) //First dialogue does not require "Return" key to be displayed
            {
                DisplayNextLine();
            }
        }
    }

    private void DisplayNextLine() //Displays dialogues, thoughts and day texts one by one from where it left.
    {
        //Debug.Log($"{currentSpeaker}: {currentLine}. On the day: {currentDay}");

        //Day index condition comes first since day lines are displayed before the dialogues.
        if (dayIndex == dialogueRoot.days.Count) //All dialogues of all days are already displayed
        {
            //Debug.Log("All dialogues of all days are already displayed");

            isTextModeOn = false;
            DialoguePanel.Instance.ChangeDialoguePanelVisibility();
            return;
        }

        if (dialogueIndex == dialogueRoot.days[dayIndex].dialogues.Count) //Last dialogue of that day
        {
            //Debug.Log("Last dialogue of that day");

            dialogueIndex = 0;
            dayIndex++;
            isDayDisplayed = false;
            willNextLineBeShown = false;
            isTextModeOn = false;

            OnDialogueDeactivated?.Invoke(this, null);

            return;
        }

        int currentDay = dialogueRoot.days[dayIndex].day;
        String currentLine = dialogueRoot.days[dayIndex].dialogues[dialogueIndex].text;
        String currentSpeaker = dialogueRoot.days[dayIndex].dialogues[dialogueIndex].speaker;
        bool currentlyIsThought = dialogueRoot.days[dayIndex].dialogues[dialogueIndex].isThought;

        if (!isDayDisplayed) //Firstly, day text is displayed
        {
            //Debug.Log("Firstly, day text is displayed");

            willNextLineBeShown = false;
            isTextModeOn = false;

            DialoguePanel.Instance.SetDayText($"Day {currentDay}");

            OnDayTextActivated?.Invoke(this, null);

            isDayDisplayed = true;
        }
        else //Any dialogue is displayed
        {
            //Debug.Log("Any dialogue is displayed");

            if (currentlyIsThought)
                DialoguePanel.Instance.SetDialogue($"{currentSpeaker}: ({currentLine})");
            else
                DialoguePanel.Instance.SetDialogue($"{currentSpeaker}: {currentLine}");

            OnDialogueActivated?.Invoke(this, null);
            dialogueIndex++;
        }
    }

    private void LogDialogues()
    {
        if (!displayed)
        {
            displayed = true;
            foreach (var day in dialogueRoot.days)
            {
                Debug.Log($"DAY {day.day}");

                foreach (var line in day.dialogues)
                {
                    Debug.Log($"line: {line}");
                    if (line.isThought)
                    {
                        Debug.Log($"({line.text})");
                    }
                    else
                    {
                        Debug.Log($"{line.speaker}: {line.text}");
                    }
                    
                    //Use yield return null to wait for one frame
                    //yield return null; // waits until next frame
                }
            }
        }
    }

    private DialogueRoot LoadDialogue()
    {
        //Debug.Log("LoadDialogue");
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName + ".json");

        if (File.Exists(path))
        {
            //Debug.Log("The path exists" + path);
            string json = File.ReadAllText(path);
            dialogueRoot = JsonUtility.FromJson<DialogueRoot>(json);
            return dialogueRoot;
        }
        else
        {
            Debug.LogError("Dialogue file not found at: " + path);
            return null;
        }
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

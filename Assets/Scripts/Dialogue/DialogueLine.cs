using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine {
    public string speaker;
    public string text;
    public bool isThought;
}

[System.Serializable]
public class DialogueDay {
    public int day;
    public List<DialogueLine> dialogues;
}

[System.Serializable]
public class DialogueRoot {
    public List<DialogueDay> days;
}

[System.Serializable]
public class DialogueProgress
{
    public int currentDay;
    public int currentLineIndex;
}
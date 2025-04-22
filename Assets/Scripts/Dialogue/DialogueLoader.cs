using UnityEngine;
using System.IO;

public class DialogueLoader : MonoBehaviour
{
    public string jsonFileName = "dialogues";

    public DialogueRoot LoadDialogue()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName + ".json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            DialogueRoot root = JsonUtility.FromJson<DialogueRoot>(json);
            return root;
        }
        else
        {
            Debug.LogError("Dialogue file not found at: " + path);
            return null;
        }
    }
}

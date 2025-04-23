using UnityEngine;

public class OnAnimationsFinish : MonoBehaviour
{
    public void EnablePlayerInteraction()
    {
        Debug.Log("Interaction script enabled again.");
        PlayerScriptsManager.Instance.EnableInteractionScript();
    }
}

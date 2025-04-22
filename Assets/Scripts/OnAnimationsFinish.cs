using UnityEngine;

public class OnAnimationsFinish : MonoBehaviour
{
    public void EnablePlayerInteraction()
    {
        PlayerScriptsManager.Instance.EnableInteractionScript();
    }
}

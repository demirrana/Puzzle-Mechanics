using UnityEngine;

public class ControlSceneSwitch : MonoBehaviour
{
    SceneManager sceneManager;

    private void Start()
    {
        sceneManager = SceneManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space is pressed.");
            sceneManager.MoveToNextScene();
        }
    }
}

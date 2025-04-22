using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { private set; get; }

    public event EventHandler OnLoadScene;
    public event EventHandler OnLoadNextDay;

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject nextDayLoadingPanel;

    private SceneNames.SceneTypeNames activeMainScene;
    private int currentDayNumber = 0;
    private SceneNames.AllSceneNames[] currentDayScenes = SceneNames.scenesOfAllDays[0];
    private SceneNames.AllSceneNames currentScene = SceneNames.AllSceneNames.FirstScene;
    private int currentSceneNumber = 0;

    private void Awake()
    {
        SetInstance();

        activeMainScene = SceneNames.SceneTypeNames.StartMenu;
    }

    private void Update()
    {
        ManageSceneChanges();
    }

    //Could be improved since the index are gonna stay the same in the state of any change of scene order.
    private int GetSceneIndexOfScene(SceneNames.SceneTypeNames sceneName)
    {
        return sceneName switch
        {
            SceneNames.SceneTypeNames.StartMenu => 0,
            SceneNames.SceneTypeNames._3DScene => 1,
            SceneNames.SceneTypeNames._2DScene => 2,
            SceneNames.SceneTypeNames.ExitMenu => 3,
            SceneNames.SceneTypeNames.OptionsMenu => 4,
            _ => 0,
        };
    }

    public void SwitchToMainScene(SceneNames.SceneTypeNames sceneType, bool isNextDay = false)
    {
        //Debug.Log("SwitchToMainScene");
        activeMainScene = sceneType;
        StartCoroutine(LoadSceneWithLoading(activeMainScene, isNextDay));
    }

    private IEnumerator LoadSceneWithLoading(SceneNames.SceneTypeNames sceneType, bool isNextDay)
    {
        Debug.Log("LoadSceneWithLoading");
        if (!isNextDay)
        {
            //Debug.Log("It is not the next day.");
            if (loadingPanel != null)
            {
                Debug.Log("loadingPanel is not null so event is triggered.");
                OnLoadScene?.Invoke(this, EventArgs.Empty); //Could be called many times!!!!!!!!!!
            }
        }
        else
        {
            if (nextDayLoadingPanel != null)
            {
                OnLoadNextDay?.Invoke(this, EventArgs.Empty); //Could be called many times!!!!!!!!!!
            }
        }

        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GetSceneIndexOfScene(sceneType));
        operation.allowSceneActivation = false; //Does not let the scene to appear immediately.

        // Wait until scene is almost loaded (90%)
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4); //Change with the time required for the loading scene (maybe make it a variable)

        operation.allowSceneActivation = true;

        yield return new WaitUntil(() => operation.isDone);

        if (operation.isDone)
        {
            Debug.Log("OP IS DONE");
        }

        if (!isNextDay)
        {
            //Debug.Log("It is AGAIN not the next day.");
            if (loadingPanel != null)
            {
                OnLoadScene?.Invoke(this, EventArgs.Empty); //Could be called many times!!!!!!!!!!
            }
        }
        else
        {
            if (nextDayLoadingPanel != null)
            {
                OnLoadNextDay?.Invoke(this, EventArgs.Empty); //Could be called many times!!!!!!!!!!
            }
        }
    }

    private SceneNames.AllSceneNames? IsTheCurrentSceneComplete()
    {
        //Debug.Log("IsTheCurrentSceneComplete");
        if (currentScene == SceneNames.AllSceneNames.TimeToSwitchTo3D)
        {
            return SceneNames.AllSceneNames.TimeToSwitchTo3D;
        }
        else if (currentScene == SceneNames.AllSceneNames.TimeToSwitchTo2D)
        {
            return SceneNames.AllSceneNames.TimeToSwitchTo2D;
        }
        else if (currentScene == SceneNames.AllSceneNames.TimeToSwitchToNextDay)
        {
            return SceneNames.AllSceneNames.TimeToSwitchToNextDay;
        }

        return null;
    }

    private void ManageSceneChanges()
    {
        //Debug.Log("ManageSceneChanges");
        switch (IsTheCurrentSceneComplete())
        {
            case SceneNames.AllSceneNames.TimeToSwitchTo3D:
                SwitchToMainScene(SceneNames.SceneTypeNames._3DScene);
                MoveToNextScene();
                return;
            case SceneNames.AllSceneNames.TimeToSwitchTo2D:
                SwitchToMainScene(SceneNames.SceneTypeNames._2DScene);
                MoveToNextScene();
                return;
            case SceneNames.AllSceneNames.TimeToSwitchToNextDay:
                SwitchToMainScene(SceneNames.SceneTypeNames._3DScene, true); //this "true" can be changed
                MoveToNextDay(); //This could be included in SwitchToMainScene method
                return;
            case null:
                return;
        }
        
    }

    //This will be called by other scripts based on the incidents happening in the game.
    public void MoveToNextScene()
    {
        Debug.Log("Scene before: " + currentScene);
        Debug.Log("MoveToNextScene");
        if (currentScene != currentDayScenes[^1]) //In case of last scene of the day, ManageSceneChanges handles the situation.
        {
            currentSceneNumber++;
            SetCurrentScene(currentDayScenes[currentSceneNumber]);
        }
        Debug.Log("Scene after: " + currentScene);
    }

    private void SetCurrentScene(SceneNames.AllSceneNames newScene)
    {
        //Debug.Log("SetCurrentScene");
        currentScene = newScene;
    }

    private void MoveToNextDay()
    {
        //Debug.Log("MoveToNextDay");
        currentDayNumber++;
        currentSceneNumber = 0;
        currentDayScenes = SceneNames.scenesOfAllDays[currentDayNumber];
        SetCurrentScene(currentDayScenes[currentSceneNumber]);
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
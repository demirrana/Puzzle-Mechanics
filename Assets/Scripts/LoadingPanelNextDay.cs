using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelNextDay : MonoBehaviour
{
    [SerializeField] private Image loadingImage;

    private void Awake()
    {
        loadingImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        SceneManager.Instance.OnLoadNextDay += ToggleActivationOfLoadingPanelNextDay;
    }

    public void ToggleActivationOfLoadingPanelNextDay(object sender, EventArgs e)
    {
        Debug.Log("ToggleActivationOfLoadingPanelNextDay");
        loadingImage.gameObject.SetActive(!loadingImage.IsActive());
    }
}

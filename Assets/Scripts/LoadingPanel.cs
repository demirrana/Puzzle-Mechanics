using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] private Image loadingImage;

    private void Awake()
    {
        loadingImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        SceneManager.Instance.OnLoadScene += ToggleActivationOfLoadingPanel;
    }

    public void ToggleActivationOfLoadingPanel(object sender, EventArgs e)
    {
        Debug.Log("ToggleActivationOfLoadingPanel");
        loadingImage.gameObject.SetActive(!loadingImage.IsActive());
    }
}

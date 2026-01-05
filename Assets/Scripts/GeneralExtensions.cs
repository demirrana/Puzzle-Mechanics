using UnityEngine;

public static class GeneralExtensions
{
    public static void Show(this GameObject gameObject)
    {
        if (gameObject.transform.childCount == 0)
        {
            gameObject.SetActive(true);
        }
        else
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public static void Hide(this GameObject gameObject)
    {
        if (gameObject.transform.childCount == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}

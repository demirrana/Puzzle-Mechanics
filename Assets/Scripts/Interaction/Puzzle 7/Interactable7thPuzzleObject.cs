using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable7thPuzzleObject : Interactable7thPuzzle
{
    private void Awake()
    {
        behavioursList.Add(new InteractableBehaviourBeChosen());

        foreach (IInteractableBehaviour b in behavioursList)
        {
            //Debug.Log(b.ToString());
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public IEnumerator FadeOut()
    {
        yield return FadeTo(0f);
        Hide();
    }

    public IEnumerator FadeIn()
    {
        Show();
        yield return FadeTo(1f);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration = 1f)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Color color = renderer.material.color;

        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            renderer.material.color = new Color(color.r, color.g, color.b, newAlpha);

            yield return null; 
        }

        renderer.material.color = new Color(color.r, color.g, color.b, targetAlpha);
    }

    public void LogBehaviours()
    {
        Debug.Log("Behaviours are:");
        foreach (IInteractableBehaviour7thPuzzle b in behavioursList)
        {
            Debug.Log(b.ToString());
        }
    }

    public void ResetBehaviours()
    {
        //Debug.Log("ResetBehaviours");
        List<IInteractableBehaviour7thPuzzle> initialBehavioursList = new();
        initialBehavioursList.Add(new InteractableBehaviourBeChosen());
        behavioursList = initialBehavioursList;
    }

    protected override void GetInteracted_Interactable(object sender, IInteractableBehaviour7thPuzzle interactionBehaviour)
    {
        //Debug.Log("Interact performed.");
        interactionBehaviour.Interact<IInteractableBehaviour7thPuzzle>(this);
        UpdateBehavioursAfter(interactionBehaviour);
    }
}

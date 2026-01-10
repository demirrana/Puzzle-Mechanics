using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable<T> : MonoBehaviour where T : IInteractableBehaviour
{
    public event EventHandler<T> OnGotInteracted;

    protected List<T> behavioursList = new();

    protected virtual void Start()
    {
        //Might be a problem since PlayerInteractionManager is disabled and enabled along the game!!!!!!!!
        //Debug.Log($"[Base Start] {name} subscribing to OnGotInteracted. InstanceID={GetInstanceID()}");
        OnGotInteracted += GetInteracted_Interactable;
    }

    public void GetInteracted(T interactionBeaviour)
    {
        //Debug.Log("GetInteracted from Interactable class is called.");
        OnGotInteracted?.Invoke(this, interactionBeaviour);
        //Debug.Log("Interaction behavior name: " + interactionBeaviour.ToString());
    }

    public void SetLayer(int layerIndex)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.layer = layerIndex;
        }

        gameObject.layer = layerIndex;
    }
    public bool CanObjectBeInteracted()
    {
        return true;
    }

    public KeyCode GetInteractionKey(T interactionBehaviour)
    {
        return interactionBehaviour.InteractionKeyCode;
    }

    public List<T> GetInteractionBehaviours()
    {
        return behavioursList;
    }

    public T GetRequestedBehaviourFromList(T sampleBehaviour)
    {
        foreach (T behaviour in behavioursList)
        {
            if (behaviour.GetType() == sampleBehaviour.GetType())
            {
                return behaviour;
            }
        }

        return default;
    }

    public bool HasBehaviour(T behaviour)
    {
        foreach (T behaviourInList in behavioursList)
        {
            if (behaviourInList.GetType() == behaviour.GetType())
            {
                return true;
            }
        }

        return false;
    }

    protected void AddBehaviour(T interactionBehaviour)
    {
        behavioursList.Add(interactionBehaviour);
    }

    protected T DeleteBehaviour(T interactionBehaviour)
    {
        var behaviour = behavioursList.FirstOrDefault(b => b is T);
        if (behaviour != null)
            behavioursList.Remove(behaviour);

        return behaviour;
    }

    protected virtual void GetInteracted_Interactable(object sender, T interactionBehaviour)
    {
        //Debug.Log($"[Base Handler] invoked on {name}, target type: {GetType().Name} (InstanceID {GetInstanceID()})");
        Debug.Log("This object is interacted by the player.");
    }

    protected void UpdatePosition(Vector3 newPosition)
    {
        gameObject.transform.position = newPosition;
    }
}
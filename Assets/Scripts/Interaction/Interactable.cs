using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event EventHandler<IInteractionBehaviour> OnGotInteracted;

    protected List<IInteractionBehaviour> behavioursList = new();

    protected virtual void Start()
    {
        //Might be a problem since PlayerInteractionManager is disabled and enabled along the game!!!!!!!!
        //Debug.Log($"[Base Start] {name} subscribing to OnGotInteracted. InstanceID={GetInstanceID()}");
        OnGotInteracted += GetInteracted_Interactable;
    }

    public void GetInteracted(IInteractionBehaviour interactionBeaviour)
    {
        //Debug.Log("GetInteracted from Interactable class is called.");
        OnGotInteracted?.Invoke(this, interactionBeaviour);
        //Debug.Log("Interaction behavior name: " + interactionBeaviour.ToString());
    }

    public bool CanObjectBeInteracted()
    {
        return true;
    }

    public KeyCode GetInteractionKey(IInteractionBehaviour interactionBehaviour)
    {
        return interactionBehaviour.InteractionKeyCode;
    }

    public List<IInteractionBehaviour> GetInteractionBehaviours()
    {
        return behavioursList;
    }

    protected void AddBehaviour(IInteractionBehaviour interactionBehaviour)
    {
        behavioursList.Add(interactionBehaviour);
    }

    protected IInteractionBehaviour DeleteBehaviour<T>(IInteractionBehaviour interactionBehaviour) where T : IInteractionBehaviour
    {
        var behaviour = behavioursList.FirstOrDefault(b => b is T);
        if (behaviour != null)
            behavioursList.Remove(behaviour);

        return behaviour;
    }

    protected virtual void GetInteracted_Interactable(object sender, IInteractionBehaviour interactionBehaviour)
    {
        //Debug.Log($"[Base Handler] invoked on {name}, target type: {GetType().Name} (InstanceID {GetInstanceID()})");
        Debug.Log("This object is interacted by the player.");
    }

    public virtual void UpdatePosition(Vector3 newPosition)
    {
        gameObject.transform.position = newPosition;
    }
}
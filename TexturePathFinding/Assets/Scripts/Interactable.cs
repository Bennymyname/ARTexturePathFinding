using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //Add or remove an InteractionEvent component to this gameobject.
    public bool useEvents;
    //message displayed to player when looking at an interactable.
    [SerializeField]
    public string promptMessage;
    // public string sceneName = "#1Stage1H";

    public virtual string OnLook()
    {
        return promptMessage;
    }
    //this function will be called from our player.
    public void BaseInteract()
    {
        if(useEvents)
        {
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        }
        Interact();
    }
    protected virtual void Interact()
    {
        //We wont have any code written in this function
        //this is a template function to be overridden by our subclasses
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : BaseMonoBehaviour
{
    public UnityEvent onInteract;
    public bool canInteract = false;

    public void Interact()
    {
        if (canInteract)
        {
            onInteract.Invoke();
        }
    }

    public void SetCanInteract(bool canInteract)
    {
        this.canInteract = canInteract;
    }
}

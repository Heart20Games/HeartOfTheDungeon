using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickupable : BaseMonoBehaviour
{
    public Interactor interactor;
    public Transform target;

    private void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }

    public void Pickup(Interactor interactor)
    {
        if (this.interactor != interactor)
        {
            this.interactor.canInteract = true;
        }
        this.interactor = interactor;
        if (this.interactor != null)
        {
            target = interactor.pickupTarget;
        }
        else
        {
            target = null;
        }
    }
}

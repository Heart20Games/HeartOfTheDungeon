using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class Triggerable : Selectable
{
    public bool togglable = false;
    public bool on = false;
    public UnityEvent onTriggeredOn;
    public UnityEvent onTriggeredOff;

    public void Trigger()
    {
        if (togglable)
        {
            if (on)
            {
                on = false;
                onTriggeredOff.Invoke();
            }
            else
            {
                on = true;
                onTriggeredOn.Invoke();
            }
        }
        else
        {
            onTriggeredOn.Invoke();
        }
    }

    // ISelectable
    public override void Confirm()
    {
        base.Confirm();
        Trigger();
    }

    
}

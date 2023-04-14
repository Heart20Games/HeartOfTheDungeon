using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ISelectable;

public class Triggerable : ASelectable
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
    public override SelectType Type { get => SelectType.Triggerable; set => NoOpSelectType(value); }

    public override void Confirm()
    {
        Trigger();
    }
}

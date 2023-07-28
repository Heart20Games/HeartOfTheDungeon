using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ISelectable;

public class Triggerable : ASelectable
{
    public bool togglable = false;
    public bool oneShot = false;
    public bool on = false;
    public UnityEvent onTriggeredOn;
    public UnityEvent onTriggeredOff;

    private bool hasFired = false;

    public void Trigger()
    {
        if (!oneShot || !hasFired)
        {
            if (togglable)
            {
                on = !on;
                if (on)
                    onTriggeredOn.Invoke();
                else
                    onTriggeredOff.Invoke();
            }
            else
            {
                onTriggeredOn.Invoke();
            }
            hasFired = true;
        }
    }

    // ISelectable
    public override SelectType Type { get => SelectType.Triggerable; set => NoOpSelectType(value); }

    public override void Confirm()
    {
        Trigger();
    }
}

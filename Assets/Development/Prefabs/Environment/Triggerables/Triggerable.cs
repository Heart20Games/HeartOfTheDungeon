using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class Triggerable : MonoBehaviour
{
    public bool togglable = false;
    public bool on = false;
    public UnityEvent onTriggeredOn;
    public UnityEvent onTriggeredOff;
    public void Trigger()
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
}

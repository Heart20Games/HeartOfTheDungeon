using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public Triggerable target;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Switch " + name + " doesn't have a target.");
        }
    }

    public void Toggle()
    {
        Debug.LogWarning("Toggle?");
        if (target != null)
        {
            Debug.LogWarning("Switch Toggled");
            target.Trigger();
        }
    }
}

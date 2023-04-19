using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    public UnityEvent onToggle;
    public List<Triggerable> targets = new List<Triggerable>();

    private void Start()
    {
        if (targets.Count == 0)
        {
            Debug.LogWarning("Switch " + name + " doesn't have a target.");
        }
    }

    public void Toggle()
    {
        onToggle.Invoke();
        foreach (Triggerable target in targets)
        {
            target.Trigger();
        }
    }
}

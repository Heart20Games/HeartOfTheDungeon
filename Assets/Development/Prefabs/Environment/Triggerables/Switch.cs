using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
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
        foreach (Triggerable target in targets)
        {
            print("Toggle");
            target.Trigger();
        }
    }
}

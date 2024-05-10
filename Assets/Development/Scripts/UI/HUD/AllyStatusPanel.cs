using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyStatusPanel : MonoBehaviour
{
    public TargetStatusDisplay displayPrefab;
    public List<TargetStatusDisplay> displayPool = new();

    private void Awake()
    {
        if (displayPool == null || displayPool.Count == 0)
        {
            displayPool = new (GetComponentsInChildren<TargetStatusDisplay>());
        }
    }

    public void AddTarget(IIdentifiable target)
    {
        // Find and use an existing display from the display pool.
        foreach (var display in displayPool)
        {
            if (display.Target == null)
            {
                display.Target = target;
                display.transform.SetAsLastSibling();
                return;
            }
        }
        // Found no empty displays to use, add a new one.
        if (displayPrefab != null)
        {
            var newDisplay = Instantiate(displayPrefab, transform);
            newDisplay.Target = target;
            displayPool.Add(newDisplay);
        }
        else Debug.LogWarning("Tried to add a new Target Character Display, found no prefab to use.");
    }

    public void RemoveTarget(IIdentifiable target)
    {
        foreach (var display in displayPool)
        {
            if (display.Target == target)
            {
                display.Target = null;
            }
        }
    }

    public void ClearTarget()
    {
        foreach (var display in displayPool)
        {
            display.Target = null;
        }
    }
}

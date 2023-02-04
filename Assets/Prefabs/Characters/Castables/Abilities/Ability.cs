using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability : MonoBehaviour, ICastable
{
    public bool followBody = true;
    public UnityEvent onCast;

    public void Cast(Vector3 direction)
    {
        onCast.Invoke();
    }
    public UnityEvent OnCasted() { return null; }
    public void Disable() { }
    public void Enable() { }
    public void Initialize(Character source) 
    {
        Transform effectOrigin = followBody ? source.body : source.transform;
        for (int l = 0; l < onCast.GetPersistentEventCount(); l++)
        {
            Component target = (Component)onCast.GetPersistentTarget(l);
            if (target is IPositionable positionable)
            {
                positionable.SetOrigin(effectOrigin, source.body);
            }
        }
    }
    public bool CanCast() { return true; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability : MonoBehaviour, ICastable
{
    public Transform effectSource;
    public UnityEvent onCast;
    public void Cast(Vector3 direction)
    {
        onCast.Invoke();
    }
    public UnityEvent OnCasted() { return null; }
    public void Disable() { }
    public void Enable() { }
    public void Initialize(Character source, Transform effectSource=null) 
    {
        this.effectSource = effectSource;
        for (int l = 0; l < onCast.GetPersistentEventCount(); l++)
        {
            Component target = (Component)onCast.GetPersistentTarget(l);
            if (target.GetType() == typeof(IPositionable))
            {
                ((IPositionable)target).SetOrigin(effectSource);
            }
        }
    }
    public bool CanCast() { return true; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability : MonoBehaviour, ICastable
{
    public void Cast(Vector3 direction) { }
    public UnityEvent OnCasted() { return null; }
    public void Disable() { }
    public void Enable() { }
    public void Initialize(Character source) { }
    public bool CanCast() { return true; }
}

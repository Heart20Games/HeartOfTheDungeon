using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ICastable
{
    /* The Castable will likely be expected to:
     * 1. position any models or effects it manages
     * 2. communicate which character animation is needed
     */
    public void Cast(Vector3 direction);
    public void UnCast();
    public UnityEvent OnCasted();
    public void Initialize(Character source);
    public void Disable();
    public void Enable();
    public bool CanCast();
    public void UnEquip();
}

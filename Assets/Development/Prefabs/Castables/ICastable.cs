using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Body;

public interface ICastable
{
    /* Castables will be expected to:
     * 1. Position any models or effects they manage
     * 2. Supply art for character animations
     * 3. Cleanup after themselves
     */
    public void Cast(Vector3 direction);
    public void UnCast();
    public UnityEvent OnCasted();
    public void Initialize(Body.Character source);
    public void Disable();
    public void Enable();
    public bool CanCast();
    public void UnEquip();
    public CastableItem GetItem();
}

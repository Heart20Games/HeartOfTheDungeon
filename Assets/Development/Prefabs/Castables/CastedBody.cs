using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CastedBody : BaseMonoBehaviour, IDamager
{
    [Serializable]
    public struct StandardEvents
    {
        public UnityEvent onStart;
        public UnityEvent onEnable;
        public UnityEvent onDisable;
    }

    public StandardEvents standardEvents;
    public UnityEvent<Impact> hitDamageable;
    public UnityEvent<Impact> leftDamageable;

    private void Start()
    {
        standardEvents.onStart.Invoke();
    }

    private void OnEnable()
    {
        standardEvents.onEnable.Invoke();
    }

    private void OnDisable()
    {
        standardEvents.onDisable.Invoke();
    }

    public void HitDamageable(Impact impact)
    {
        hitDamageable.Invoke(impact);
    }

    public void LeftDamageable(Impact impact)
    {
        leftDamageable.Invoke(impact);
    }
}

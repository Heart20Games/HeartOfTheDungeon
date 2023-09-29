using System;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace HotD.Castables
{
    public class Casted : Positionable, ICollidables
    {
        [Header("Base Events")]
        public UnityEvent onStart = new();
        public UnityEvent onEnable = new();
        public UnityEvent onDisable = new();

        [Header("Castable Events")]
        public UnityEvent onTrigger = new();
        public UnityEvent onRelease = new();
        public UnityEvent<Vector3> onCast = new();
        public UnityEvent onUnCast = new();
        public void Trigger() { onTrigger.Invoke(); }
        public void Release() { onRelease.Invoke(); }
        public void Cast(Vector3 vector) { onCast.Invoke(vector); }
        public void UnCast() { onUnCast.Invoke(); }

        [Header("Power Level")]
        public UnityEvent<float> onSetPowerLevel = new();
        public UnityEvent<int> onSetPowerLimit = new();
        public void SetPowerLevel(float powerLevel) { onSetPowerLevel.Invoke(powerLevel); }
        public void SetPowerLimit(int powerLimit) { onSetPowerLimit.Invoke(powerLimit); }

        public UnityEvent<Collider[]> onSetExceptions = new();

        private void Start()
        {
            onStart.Invoke();
        }
        private void OnEnable()
        {
            onEnable.Invoke();
        }
        private void OnDisable()
        {
            onDisable.Invoke();
        }

        public void SetExceptions(Collider[] exceptions)
        {
            onSetExceptions.Invoke(exceptions);
        }
    }
}
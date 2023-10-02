using UnityEngine;
using UnityEngine.Events;
using Pixeye.Unity;
using System.Collections.Generic;
using static CastableStats;
using System;

namespace HotD.Castables
{
    public class Casted : Positionable, ICollidables
    {
        public CastableStats stats;

        [Foldout("Base Events", true)]
        public UnityEvent onStart = new();
        public UnityEvent onEnable = new();
        public UnityEvent onDisable = new();

        [Foldout("Cast Events", true)]
        public UnityEvent onTrigger = new();
        public UnityEvent onRelease = new();
        public UnityEvent<Vector3> onCast = new();
        public UnityEvent onUnCast = new();
        public void Trigger() { onTrigger.Invoke(); }
        public void Release() { onRelease.Invoke(); }
        public void Cast(Vector3 vector) { onCast.Invoke(vector); }
        public void UnCast() { onUnCast.Invoke(); }

        [Foldout("Power Level")]
        public UnityEvent<float> onSetPowerLevel = new();
        [Foldout("Power Level")]
        public UnityEvent<int> onSetPowerLimit = new();

        public UnityEvent<Collider[]> onSetExceptions = new();

        [Serializable] public struct CastStatEvent
        {
            public string name;
            public CastableStat stat;
            public float modifier;
            public float multiplier;
            public UnityEvent<float> finalValue;
        }
        public List<CastStatEvent> castStatEvents = new();

        private void Start()
        {
            ReportStats();
            onStart.Invoke();
        }
        private void OnEnable()
        {
            ReportStats();
            onEnable.Invoke();
        }
        private void OnDisable()
        {
            onDisable.Invoke();
        }

        public void ReportStats()
        {
            foreach (var e in castStatEvents)
            {
                e.finalValue.Invoke((stats.GetAttribute(e.stat).FinalValue + e.modifier) * e.multiplier);
            }
        }

        public void SetExceptions(Collider[] exceptions)
        {
            onSetExceptions.Invoke(exceptions);
        }

        // Power Level
        public void SetPowerLevel(float powerLevel)
        {
            onSetPowerLevel.Invoke(powerLevel);
        }
        public void SetPowerLimit(int powerLimit)
        {
            onSetPowerLimit.Invoke(powerLimit);
        }

        // Cast Events
        public void OnTrigger()
        {
            onTrigger.Invoke();
        }
        public void OnRelease()
        {
            onRelease.Invoke();
        }
        public void OnCast(Vector3 vector)
        {
            onCast.Invoke(vector);
        }
        public void OnUnCast()
        {
            onUnCast.Invoke();
        }
    }
}
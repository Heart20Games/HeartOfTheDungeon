using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    public class CastListenerDistributor : ACastListener
    {
        [SerializeField] private List<ICastListener> listeners = new();
        [SerializeField][ReadOnly] private int listenerCount = 0;
        [SerializeField] private bool debugDistribution;
        public CastProperties properties;

        private int ListenerCount { get { listenerCount = listeners.Count; return listenerCount; } }

        private void Awake()
        {
            ICastListener[] listeners = GetComponentsInChildren<ICastListener>(true);
            foreach (var listener in listeners)
            {
                if (listener == (this as ICastListener) || this.listeners.Contains(listener))
                {
                    continue;
                }
                AddListener(listener);
            }
        }

        public void AddListener(ICastListener listener)
        {
            listeners.Add(listener);
            Print($"Adding listener (now has {ListenerCount}).", true, this);
        }

        public override void SetChargeTimes(float[] times)
        {
            Print($"Setting charge times on {ListenerCount} listeners.", debugDistribution, this);
            foreach (var listener in listeners)
            {
                if (listener != null)
                    listener.ChargeTimes = times;
                else
                    Print("Found null listener on CastListenerDistributor.", debugDistribution, this);
            }
        }

        public override void SetLevel(int level)
        {
            Print($"Setting level on {ListenerCount} listeners.", debugDistribution, this);
            foreach (var listener in listeners)
            {
                listener.Level = level;
            }
        }

        public override void SetOwner(ICastCompatible owner)
        {
            Print($"Setting owner on {ListenerCount} listeners.", debugDistribution, this);
            foreach (var listener in listeners)
            {
                listener.Owner = owner;
            }
        }

        public override void SetTriggers(Coordination.Triggers triggers)
        {
            Print($"Setting Triggers on {ListenerCount} listeners.", debugDistribution, this);
            foreach (var listener in listeners)
            {
                listener.SetTriggers(triggers);
            }
        }
    }
}
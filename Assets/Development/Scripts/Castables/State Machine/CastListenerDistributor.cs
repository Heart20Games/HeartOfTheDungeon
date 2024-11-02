using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    /*
     * The Cast Listener Distributor simply receives the typical Cast Listener calls then passes them along to a list of child listeners.
     * 
     * It's function is primarily organizational; this allows us to configure all of our Cast Listener event connections once, then pass them along to an arbitrarily long list of children.
     */

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

        public override void ChargeTimesSet(float[] times)
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

        public override void LevelSet(int level)
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
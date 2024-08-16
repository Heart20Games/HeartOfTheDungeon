using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    public class CastListenerDistributor : ACastListener
    {
        [SerializeField] private List<ACastListener> listeners = new();
        public CastProperties properties;

        public void AddListener(ACastListener listener)
        {
            listeners.Add(listener);
        }

        public override void SetChargeTimes(float[] times)
        {
            foreach (var listener in listeners)
            {
                listener.ChargeTimes = times;
            }
        }

        public override void SetLevel(int level)
        {
            foreach (var listener in listeners)
            {
                listener.Level = level;
            }
        }

        public override void SetOwner(ICastCompatible owner)
        {
            foreach (var listener in listeners)
            {
                listener.Owner = owner;
            }
        }

        public override void SetTriggers(Coordination.Triggers triggers)
        {
            foreach (var listener in listeners)
            {
                listener.SetTriggers(triggers);
            }
        }
    }
}
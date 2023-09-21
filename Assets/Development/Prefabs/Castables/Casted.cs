using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class Casted : BaseMonoBehaviour
    {
        public StandardEvents standardEvents;
        [Serializable] public struct StandardEvents
        {
            public UnityEvent onStart;
            public UnityEvent onEnable;
            public UnityEvent onDisable;
        }

        public CastableEvents castableEvents;
        [Serializable] public struct CastableEvents
        {
            public UnityEvent<float> onSetPowerLevel;
            public UnityEvent<int> onSetPowerLimit;
            public void Connect(Castable castable)
            {

            }
        }

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
    }
}
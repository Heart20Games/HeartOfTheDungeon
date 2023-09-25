using System;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using UnityEngine.Assertions;

namespace HotD.Castables
{
    public class Casted : BaseMonoBehaviour
    {
        public UnityEvent onStart = new();
        public UnityEvent onEnable = new();
        public UnityEvent onDisable = new();

        public UnityEvent<float> onSetPowerLevel = new();
        public UnityEvent<int> onSetPowerLimit = new();

        public void SetPowerLevel(float powerLevel)
        {
            onSetPowerLevel.Invoke(powerLevel);
        }

        public void SetPowerLimit(int powerLimit)
        {
            onSetPowerLimit.Invoke(powerLimit);
        }

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
    }
}
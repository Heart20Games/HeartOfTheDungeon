using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class ExecutionMethod : CastableProperties
    {
        public UnityEvent onEnable;

        private void OnEnable()
        {
            onEnable.Invoke();
        }
    }
}

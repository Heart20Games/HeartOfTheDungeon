using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace HotD.Castables
{
    using static Coordination;

    public interface ICastListener
    {
        public float[] ChargeTimes { get; set; }
        public int Level { get; set; }
        public void SetChargeTimes(float[] times);
        public void SetLevel(int level);
        public void SetTriggers(Triggers triggers);
    }

    public abstract class CastListener : BaseMonoBehaviour, ICastListener
    {
        public virtual float[] ChargeTimes { get => null; set => SetChargeTimes(value); }
        public virtual int Level { get => 0; set => NULL(); }

        public abstract void SetChargeTimes(float[] times);
        public abstract void SetLevel(int level);
        public abstract void SetTriggers(Triggers triggers);
    }
}
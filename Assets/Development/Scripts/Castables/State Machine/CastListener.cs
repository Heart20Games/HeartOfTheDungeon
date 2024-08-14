using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace HotD.Castables
{
    using static Coordination;

    public interface ICastListener
    {
        public ICastCompatible Owner { get; set; }
        public int Level { get; set; }
        public float[] ChargeTimes { get; set; }
        public void SetOwner(ICastCompatible owner);
        public void SetLevel(int level);
        public void SetChargeTimes(float[] times);
        public void SetTriggers(Triggers triggers);
    }

    public abstract class CastListener : BaseMonoBehaviour, ICastListener
    {
        protected ICastCompatible owner;

        public virtual ICastCompatible Owner { get => owner; set => SetOwner(value); }
        public virtual int Level { get => 0; set => SetLevel(value); }
        public virtual float[] ChargeTimes { get => null; set => SetChargeTimes(value); }

        public virtual void SetOwner(ICastCompatible owner) { this.owner = owner; }
        public abstract void SetLevel(int level);
        public abstract void SetChargeTimes(float[] times);
        public abstract void SetTriggers(Triggers triggers);
    }
}
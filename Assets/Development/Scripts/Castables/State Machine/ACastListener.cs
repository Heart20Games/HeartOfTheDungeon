
namespace HotD.Castables
{
    using UnityEngine;
    using static Coordination;

    public interface ICastListener: ICastCompatibleFollower
    {
        public int Level { get; set; }
        public float[] ChargeTimes { get; set; }
        public void SetLevel(int level);
        public void SetChargeTimes(float[] times);
        public void SetTriggers(Triggers triggers);
    }

    public abstract class ACastListener : ACastCompatibleFollower, ICastListener
    {
        public virtual int Level { get => 0; set => SetLevel(value); }
        public virtual float[] ChargeTimes { get => null; set => SetChargeTimes(value); }

        public abstract void SetLevel(int level);
        public abstract void SetChargeTimes(float[] times);
        public abstract void SetTriggers(Triggers triggers);
    }

    public class CastListener : ACastListener
    {
        [SerializeField] private int level;
        [SerializeField] private float[] chargeTimes;

        public override void SetChargeTimes(float[] times)
        {
            chargeTimes = times;
        }

        public override void SetLevel(int level)
        {
            this.level = level;
        }

        public override void SetTriggers(Triggers triggers)
        {
            return;
        }
    }
}
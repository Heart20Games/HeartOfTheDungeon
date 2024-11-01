
namespace HotD.Castables
{
    using FMODUnity;
    using UnityEngine;
    using static Coordination;

    public interface ICastChargeListener
    {
        public int Level { get; set; }
        public float[] ChargeTimes { get; set; }
        public void SetLevel(int level);
        public void SetChargeTimes(float[] times);
    }

    public interface ICastTriggerListener
    {
        public void SetTriggers(Triggers triggers);
    }

    public interface ICastListener: ICastCompatibleFollower, ICastChargeListener, ICastTriggerListener
    {
        //public void SetActive(bool active);
    }

    public abstract class ACastListener : ACastCompatibleFollower, ICastListener
    {
        //public virtual void SetActive(bool active) { Debug.LogWarning("SetActive not overridden."); }
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

        private StudioEventEmitter eventEmitter;
        //public override void SetActive(bool active) { /* We don't actually want to do anything. */ }

        public override void SetChargeTimes(float[] times)
        {
            chargeTimes = times;
        }

        public override void SetLevel(int level)
        {
            this.level = level;
            eventEmitter = GetComponent<StudioEventEmitter>();
            eventEmitter.SetParameter("MagicBoltLevel", level);
        }

        public override void SetTriggers(Triggers triggers)
        {
            return;
        }
    }
}
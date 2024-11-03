
namespace HotD.Castables
{
    using FMODUnity;
    using MyBox;
    using UnityEngine;
    using UnityEngine.Events;
    using static Coordination;

    // Interfaces

    public interface ICastChargeListener
    {
        public int Level { get; set; }
        public float[] ChargeTimes { get; set; }
        public void LevelSet(int level);
        public void ChargeTimesSet(float[] times);
    }

    public interface ICastTriggerListener
    {
        public void SetTriggers(Triggers triggers);
    }

    /*
     * Cast Listeners are intended to be persistently enabled objects that need to receive information about the status of the Cast State Machine (CSM).
     * 
     * What do they receive? See the Interfaces above.
     * 
     * When would you use this? VFX, Sound Effects, anything that needs to change and persist between Cast States.
     */

    public interface ICastListener: ICastCompatibleFollower, ICastChargeListener, ICastTriggerListener { }

    public abstract class ACastListener : ACastCompatibleFollower, ICastListener
    {
        public virtual int Level { get => 0; set => LevelSet(value); }
        public virtual float[] ChargeTimes { get => null; set => ChargeTimesSet(value); }

        public abstract void LevelSet(int level);
        public abstract void ChargeTimesSet(float[] times);
        public abstract void SetTriggers(Triggers triggers);
    }

    public class CastListener : ACastListener
    {
        [SerializeField] protected int level;
        [SerializeField] protected float[] chargeTimes;

        public override void ChargeTimesSet(float[] times)
        {
            chargeTimes = times;
        }

        public override void LevelSet(int level)
        {
            this.level = level;
        }

        public override void SetTriggers(Triggers triggers)
        {
            return;
        }
    }
}
using MyBox;
using UnityEngine;

namespace HotD.PostProcessing
{
    using static PostProcessor;

    public enum PType { None, Death }

    public class VolumeManager : BaseMonoBehaviour
    {
        public DeathProcessor death;
        public bool dead;

        public void Awake()
        {
            if (death == null)
                death = GetComponent<DeathProcessor>();
        }

        public PStatus Status(PType type)
        {
            return type switch
            {
                PType.Death => death.status,
                _ => PostProcessor.PStatus.None,
            };
        }
        public PStatus Target(PType type)
        {
            return type switch
            {
                PType.Death => death.targetStatus,
                _ => PostProcessor.PStatus.None,
            };
        }
        public PStatus Active(bool active)
        {
            return active ? PStatus.Active : PStatus.Inactive;
        }


        public bool IsTransitioning(PType type, bool toTarget=false, bool active=false)
        {
            if (toTarget)
                return Status(type) == PStatus.Transitioning && Target(type) == Active(active);
            else
                return Status(type) == PStatus.Transitioning;
        }
        public bool IsActive(PType type)
        {
            return Status(type) == PostProcessor.PStatus.Active;
        }
        public bool IsInactive(PType type)
        {
            return Status(type) == PostProcessor.PStatus.Inactive;
        }

        [ButtonMethod]
        public void SpeedUp()
        {
            SpeedUp(PType.Death);
        }
        public void SpeedUp(PType type)
        {
            switch (type)
            {
                case PType.Death:
                    if (death != null) death.SpeedUp(); break;
            }
        }

        [ButtonMethod] public void MakeDead() { SetDeath(true); }
        [ButtonMethod] public void UnMakeDead() { SetDeath(false); }

        public void SetDeath(bool die)
        {
            death.SetActive(die);
            dead = die;
        }
    }
}

using MyBox;
using UnityEngine;

namespace HotD.PostProcessing
{
    using static PostProcessor;

    public enum ProcessorType { None, Death }

    public class VolumeManager : BaseMonoBehaviour
    {
        public DeathProcessor death;
        public bool dead;

        public void Awake()
        {
            if (death == null)
                death = GetComponent<DeathProcessor>();
        }

        public bool IsActive(ProcessorType type)
        {
            return type switch
            {
                ProcessorType.Death => death.status == Status.Active,
                _ => false,
            };
        }

        [ButtonMethod]
        public void ToggleDeath()
        {
            ToggleDeath(!dead);
        }

        public void ToggleDeath(bool die)
        {
            if (dead != die)
            {
                switch (die)
                {
                    case true: death.Activate(); break;
                    case false: death.Deactivate(); break;
                }
            }
            dead = die;
        }
    }
}

using MyBox;
using UnityEngine;

namespace HotD.PostProcessing
{
    public class GlobalVolumeManager : MonoBehaviour
    {
        public DeathProcessor death;
        public bool dead;

        public void Awake()
        {
            if (death == null)
                death = GetComponent<DeathProcessor>();
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

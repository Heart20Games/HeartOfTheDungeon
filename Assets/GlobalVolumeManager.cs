using MyBox;
using UnityEngine;

public class GlobalVolumeManager : MonoBehaviour
{
    public DeathPostProcessing death;
    public bool dead;

    public void Awake()
    {
        if (death == null)
            death = GetComponent<DeathPostProcessing>();
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
                case true: death.Death(); break;
                case false: death.Alive(); break;
            }
        }
        dead = die;
    }
}

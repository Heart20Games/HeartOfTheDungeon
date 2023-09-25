using System;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class CastedCollider : Casted, IDamager
    {
        public UnityEvent<Impact> hitDamageable;
        public UnityEvent<Impact> leftDamageable;

        public void HitDamageable(Impact impact)
        {
            hitDamageable.Invoke(impact);
        }

        public void LeftDamageable(Impact impact)
        {
            leftDamageable.Invoke(impact);
        }
    }
}

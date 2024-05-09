using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public interface IDamager
{
    public void HitDamageable(Impact impactor);
    public void LeftDamageable(Impact impactor);
}

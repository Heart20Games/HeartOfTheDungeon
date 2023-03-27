using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability : Castable
{
    public bool followBody = true;

    public override void Initialize(Character source) 
    {
        base.Initialize(source);
        Transform effectOrigin = followBody ? source.body : source.transform;
        for (int l = 0; l < onCast.GetPersistentEventCount(); l++)
        {
            Object target = onCast.GetPersistentTarget(l);
            if (target is IPositionable positionable)
            {
                positionable.SetOrigin(effectOrigin, source.body);
            }
        }
    }
}

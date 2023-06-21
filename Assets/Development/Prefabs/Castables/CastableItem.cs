using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSContext;
using static Body.Behavior.ContextSteering.CSIdentity;

[CreateAssetMenu(fileName = "NewCastableItem", menuName = "Loadouts/CastableItem", order = 1)]
public class CastableItem : ScriptableObject
{
    public Castable prefab;
    public Context context = new();
}

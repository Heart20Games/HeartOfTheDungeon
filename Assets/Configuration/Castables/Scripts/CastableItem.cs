using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSContext;

[CreateAssetMenu(fileName = "NewCastableItem", menuName = "Loadouts/CastableItem", order = 1)]
public class CastableItem : ScriptableObject
{
    public Castable prefab;
    public int attackIdx = 0;
    public CastableStats stats;
    public Context context = new();

    public void UnEquip()
    {
        stats.UnEquip();
    }

    public void Equip(StatBlock statBlock)
    {
        if (stats != null)
            stats.Equip(statBlock);
    }
}

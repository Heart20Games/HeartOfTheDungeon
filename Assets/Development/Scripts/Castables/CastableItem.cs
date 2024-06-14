using UnityEngine;
using static Body.Behavior.ContextSteering.CSContext;

namespace HotD.Castables
{
    public enum TargetingMethod { None, TargetBased, LocationBased, DirectionBased, AimBased }
    public enum AimingMethod { Centered, OverTheShoulder }

    [CreateAssetMenu(fileName = "NewCastableItem", menuName = "Loadouts/CastableItem", order = 1)]
    public class CastableItem : ScriptableObject
    {
        public Castable prefab;
        public int attackIdx = 0;
        public TargetingMethod targetingMethod;
        public AimingMethod aimingMethod;
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
}
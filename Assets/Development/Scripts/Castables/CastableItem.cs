using UnityEngine;
using static Body.Behavior.ContextSteering.CSContext;

namespace HotD.Castables
{
    public enum TargetingMethod { None, TargetBased, LocationBased, DirectionBased, AimBased }
    public enum AimingMethod { Centered, OverTheShoulder }
    public enum ActionType { Passive, Staff, Daggers, Rapier, Throw, Sabre, Charge, Spear }

    [CreateAssetMenu(fileName = "NewCastableItem", menuName = "Loadouts/CastableItem", order = 1)]
    public class CastableItem : ScriptableObject
    {
        public GameObject prefab;
        public ICastable Prefab { get => prefab.GetComponent<ICastable>(); }
        public int attackIdx = 0;
        public ActionType actionType = ActionType.Passive;
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
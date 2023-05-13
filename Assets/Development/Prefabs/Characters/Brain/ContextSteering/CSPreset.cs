using UnityEngine;
using ScriptableObjectDropdown;
using System.Collections.Generic;
using System.Data;

namespace Body.Behavior.ContextSteering
{
    using static CSIdentity;
    using static CSContext;
    using static CSMapping;

    [CreateAssetMenu(fileName = "CSPreset", menuName = "Context Steering/Preset", order = 1)]
    public class CSPreset : ScriptableObject
    {
        public float testSpeed = 3f;
        public float drawScale = 1.0f;
        public bool draw;

        [ScriptableObjectDropdown(typeof(CSContext), grouping = ScriptableObjectGrouping.ByFolderFlat)]
        public ScriptableObjectReference contextPreset;
        [ScriptableObjectDropdown(typeof(CSIdentity), grouping = ScriptableObjectGrouping.ByFolderFlat)]
        public ScriptableObjectReference identityPreset;

        //public Contexts Contexts { get { return ((CSContext)contextPreset.value).contexts; } }
        public Dictionary<Identity, Context> Contexts { get { return ((CSContext) contextPreset.value).contextMap; } }
        public Context GetContext(Identity id) { return ((CSContext)contextPreset.value)[id]; }
        public Identity Identity { get { return ((CSIdentity)identityPreset.value).identity; } }
        public IdentityMapPair[] Pairs { get { return ((CSIdentity)identityPreset.value).pairs; } }
        public Dictionary<Identity, IdentityMapPair> IdentityMap { get { return ((CSIdentity)identityPreset.value).IdentityMap; } }
    }
}
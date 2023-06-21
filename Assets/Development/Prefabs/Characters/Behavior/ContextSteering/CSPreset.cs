using UnityEngine;
using ScriptableObjectDropdown;
using System.Collections.Generic;
using System.Data;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Body.Behavior.ContextSteering
{
    using static CSIdentity;
    using static CSContext;
    using static CSMapping;
    using static Brain;

    [CreateAssetMenu(fileName = "CSPreset", menuName = "Context Steering/Preset", order = 1)]
    public class CSPreset : ScriptableObject
    {
        public float scale = 1f;
        public float testSpeed = 3f;
        public float drawScale = 1.0f;
        public bool draw;

        public Identity identity = Identity.Neutral;

        [ScriptableObjectDropdown(typeof(CSContext), grouping = ScriptableObjectGrouping.ByFolderFlat)]
        public ScriptableObjectReference chaseContext;
        [ScriptableObjectDropdown(typeof(CSContext), grouping = ScriptableObjectGrouping.ByFolderFlat)]
        public ScriptableObjectReference duelContext;
        [ScriptableObjectDropdown(typeof(CSIdentity), grouping = ScriptableObjectGrouping.ByFolderFlat)]
        public ScriptableObjectReference chaseIdentity;
        [ScriptableObjectDropdown(typeof(CSIdentity), grouping = ScriptableObjectGrouping.ByFolderFlat)]
        public ScriptableObjectReference duelIdentity;

        public CSContext GetContext(Action action, bool debug=false)
        {
            ScriptableObjectReference reference = action switch
            {
                Action.Chase => chaseContext,
                Action.Duel => duelContext,
                _ => null
            };
            return reference == null ? null : (CSContext)reference.value;
        }

        public CSIdentity GetRelationships(Action action)
        {
            ScriptableObjectReference reference = action switch
            {
                Action.Chase => chaseIdentity,
                Action.Duel => duelIdentity,
                _ => null
            };
            return reference == null ? null : (CSIdentity)reference.value;
        }

        public Identity Identity => identity;
    }
}
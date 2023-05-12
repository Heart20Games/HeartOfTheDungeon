using System;
using System.Collections.Generic;
using UnityEngine;

namespace Body.Behavior.ContextSteering
{
    using static CSMapping;

    [CreateAssetMenu(fileName = "CSIdentity", menuName = "Context Steering/Identity", order = 1)]
    public class CSIdentity : ScriptableObject
    {
        public enum Identity { Neutral, Friend, Foe }

        public Identity identity = Identity.Neutral;
        public IdentityMapPair[] pairs = new IdentityMapPair[] {
            new(Identity.Neutral, MapType.None, 0f),
            new IdentityMapPair(Identity.Foe, MapType.Interest, 1f),
            new IdentityMapPair(Identity.Friend, MapType.Danger, 0.5f)
        };


        static readonly public IdentityMapPair[] defaultPairs = new IdentityMapPair[]
        {
            new(Identity.Neutral, MapType.None, 0f),
            new IdentityMapPair(Identity.Foe, MapType.Interest, 1f),
            new IdentityMapPair(Identity.Friend, MapType.Danger, 0.5f)
        };

        public Dictionary<Identity, IdentityMapPair> identityMap = null;
        public Dictionary<Identity, IdentityMapPair> IdentityMap
        {
            get
            {
                if (identityMap == null)
                {
                    identityMap = new();
                    foreach (var pair in pairs)
                    {
                        identityMap[pair.identity] = pair;
                    }
                }
                return identityMap; 
            }
        }

        [Serializable]
        public struct IdentityMapPair
        {
            public IdentityMapPair(Identity identity, MapType mapType, float weight)
            {
                name = identity.ToString();
                this.identity = identity;
                this.mapType = mapType;
                this.weight = weight;
            }
            public string name;
            public Identity identity;
            public MapType mapType;
            public float weight;
        }
    }
}
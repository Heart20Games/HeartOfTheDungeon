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
            new(Identity.Neutral, MapType.None),
            new IdentityMapPair(Identity.Foe, MapType.Interest),
            new IdentityMapPair(Identity.Friend, MapType.Danger)
        };


        static readonly public IdentityMapPair[] defaultPairs = new IdentityMapPair[]
        {
            new(Identity.Neutral, MapType.None),
            new IdentityMapPair(Identity.Foe, MapType.Interest),
            new IdentityMapPair(Identity.Friend, MapType.Danger)
        };

        public Dictionary<Identity, MapType> identityMap = null;
        public Dictionary<Identity, MapType> IdentityMap
        {
            get
            {
                if (identityMap == null)
                {
                    identityMap = new();
                    foreach (var pair in pairs)
                    {
                        identityMap[pair.identity] = pair.mapType;
                    }
                }
                return identityMap; 
            }
        }

        [Serializable]
        public struct IdentityMapPair
        {
            public IdentityMapPair(Identity identity, MapType mapType)
            {
                name = identity.ToString();
                this.identity = identity;
                this.mapType = mapType;
            }
            public string name;
            public Identity identity;
            public MapType mapType;
        }
    }
}
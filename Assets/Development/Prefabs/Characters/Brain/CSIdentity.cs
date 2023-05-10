using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CSMapping;

[CreateAssetMenu(fileName = "CSIdentity", menuName = "Context Steering/Identity", order = 1)]
public class CSIdentity : ScriptableObject
{
    public Identity identity = Identity.Neutral;
    public IdentityMapPair[] pairs = defaultPairs;

    public enum Identity { Neutral, Friend, Foe }

    static readonly public IdentityMapPair[] defaultPairs = new IdentityMapPair[]
    {
        new(Identity.Neutral, MapType.None),
        new IdentityMapPair(Identity.Foe, MapType.Interest),
        new IdentityMapPair(Identity.Friend, MapType.Danger)
    };

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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ContextSteeringController;

public class ContextSteeringStructs
{
    // Enums
    public enum Identity { Neutral, Friend, Foe }
    public enum ContextType { Peer, Target, Obstacle }
    public enum MapType { Interest, Danger, None }

    // Defaults
    static readonly public Context[] defaultContexts = new Context[]
    {
        new(ContextType.Peer, 1f, 0f, 5f, 20f),
        new (ContextType.Target, 1f, 0f, 1000f, 20f),
        new (ContextType.Obstacle, 1f, 0f, 5f, 20f),
    };
    static readonly public IdentityMapPair[] defaultPairs = new IdentityMapPair[]
    {
        new(Identity.Neutral, MapType.None),
        new IdentityMapPair(Identity.Foe, MapType.Interest),
        new IdentityMapPair(Identity.Friend, MapType.Danger)
    };

    // Resolution
    static public int resolution = 12;
    static private Vector2[] baseline;
    static public Vector2[] Baseline
    {
        get { return baseline ?? InitializeBaseline(); }
        set => baseline = value;
    }
    static private Vector2[] InitializeBaseline()
    {
        baseline = new Vector2[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float angle = Mathf.Lerp(0, 360, i / resolution);
            baseline[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        return baseline;
    }

    [Serializable]
    public struct Context
    {
        public Context(ContextType type, float weight, float minDistance, float maxDistance, float falloff)
        {
            name = type.ToString();
            this.type = type;
            this.weight = weight;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.falloff = falloff;
        }
        public string name;
        public ContextType type;
        public float weight;
        public float minDistance;
        public float maxDistance;
        public float falloff;
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

    public class IdentityComparer : IEqualityComparer<Identity>
    {
        public bool Equals(Identity x, Identity y)
        {
            return x == y;
        }

        public int GetHashCode(Identity x)
        {
            return (int)x;
        }
    }
}

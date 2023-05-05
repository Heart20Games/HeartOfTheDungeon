using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static ContextSteeringController;

public class ContextSteeringStructs
{
    // Enums
    public enum Identity { Neutral, Friend, Foe }
    public enum ContextType { Peer, Target, Obstacle, None }
    public enum MapType { Interest, Danger, None }

    // Defaults
    static readonly public Contexts defaultContexts = new(
        new(ContextType.Peer, 1f, 0f, 5f, 50f),
        new(ContextType.Target, 1f, 0f, 1000f, 50f),
        new(ContextType.Obstacle, 1f, 0f, 5f, 50f)
    );
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
            float angle = Mathf.Lerp(0, 360, (float)i / resolution);
            baseline[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        PrintVectors(baseline, "Baseline");

        return baseline;
    }

    [Serializable]
    public struct Contexts
    {
        public Contexts(Context peer, Context target, Context obstacle)
        {
            this.peer = peer;
            this.target = target;
            this.obstacle = obstacle;
        }
        public Context peer;
        public Context target;
        public Context obstacle;
        public Context GetContext(ContextType type)
        {
            switch (type)
            {
                case ContextType.Peer: return peer;
                case ContextType.Target: return target;
                case ContextType.Obstacle: return obstacle;
                default: return new(ContextType.None, 0f, 0f, 0f, 0f);
            }
        }
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

    [Serializable]
    public struct Maps
    {
        public Maps(float[] interests, float[] dangers)
        {
            this.interests = interests;
            this.dangers = dangers;
        }
        public float[] interests;
        public float[] dangers;
        public float[] GetMap(MapType type)
        {
            switch (type)
            {
                case MapType.Interest: return interests;
                case MapType.Danger: return dangers;
                default: return null;
            }
        }
    }

    //public class IdentityComparer : IEqualityComparer<Identity>
    //{
    //    public bool Equals(Identity x, Identity y)
    //    {
    //        return x == y;
    //    }

    //    public int GetHashCode(Identity x)
    //    {
    //        return (int)x;
    //    }
    //}

    static public void PrintVectors(Vector2[] vectors, string label)
    {
        StringBuilder b = new();
        b.Append(label);
        b.Append(" ");
        b.AppendJoin(", ", vectors);
        Debug.Log(b);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static ContextSteeringController;

public class ContextSteeringStructs
{
    // Enums
    public enum Identity { Neutral, Friend, Foe }
    public enum ContextType { Peer, Target, Obstacle, None }
    public enum MapType { Interest, Danger, None }
    const sbyte POS = 1;
    const sbyte NA = 0;
    const sbyte NEG = -1;

    // Defaults
    static readonly public Contexts defaultContexts = new(
        new(ContextType.Peer, 1f, 0f, 5f, -1f, 50f),
        new(ContextType.Target, 1f, 0f, 1000f, -1f, 50f),
        new(ContextType.Obstacle, 1f, 0f, 5f, -1f, 50f)
    );
    static readonly public IdentityMapPair[] defaultPairs = new IdentityMapPair[]
    {
        new(Identity.Neutral, MapType.None),
        new IdentityMapPair(Identity.Foe, MapType.Interest),
        new IdentityMapPair(Identity.Friend, MapType.Danger)
    };

    // Resolution
    static public int resolution = 12;
    static private Vector3[] baseline;
    static public Vector3[] Baseline
    {
        get { return baseline ?? InitializeBaseline(); }
        set => baseline = value;
    }
    static private Vector3[] InitializeBaseline()
    {
        baseline = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float angle = Mathf.Lerp(0, 360, (float)i / resolution);
            baseline[i] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
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
                default: return new(ContextType.None, 0f, 0f, 0f, -1f, 0f);
            }
        }
    }

    [Serializable]
    public struct Context
    {
        public Context(ContextType type, float weight, float minDistance, float maxDistance, float cullDistance, float falloff)
        {
            name = type.ToString();
            this.type = type;
            this.weight = weight;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.cullDistance = cullDistance >= 0f ? cullDistance : float.MaxValue;
            this.falloff = falloff;
        }
        public string name;
        public ContextType type;
        public float weight;
        public float minDistance;
        public float maxDistance;
        public float cullDistance;
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
    public struct Map
    {
        public Map(float[] dirs, sbyte sign)
        {
            this.dirs = dirs ?? new float[resolution];
            this.sign = sign;
            this.valid = true;
        }
        public Map(float[] dirs, sbyte sign, bool initialize)
        {
            this.dirs = !initialize ? dirs : (dirs ?? new float[resolution]);
            this.sign = sign;
            this.valid = true;
        }
        //public float[] dirs;
        public float[] dirs;
        public sbyte sign;
        public bool valid;
        public int Length => dirs.Length;
        public float this[int index]
        {
            get { return dirs[index]; }
            set { dirs[index] = value; }
        }
        public new string ToString()
        {
            return dirs.ToString();
        }
    }

    [Serializable]
    public struct Maps
    {
        public Maps(Map interests, Map dangers)
        {
            this.interests = interests;
            this.dangers = dangers;
        }
        public Maps(float[] interests, float[] dangers)
        {
            this.interests = new(interests, POS);
            this.dangers = new(dangers, NEG);
        }
        public Map interests;
        public Map dangers;
        public int Length => 2;
        public Map this[int index]
        {
            get
            {
                return index switch
                {
                    0 => interests,
                    1 => dangers,
                    _ => new(),
                };
            }
        }
        public Map this[MapType type]
        {
            get
            {
                return type switch
                {
                    MapType.Interest => interests,
                    MapType.Danger => dangers,
                    _ => new(),
                };
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

    static public void PrintVectors(Vector3[] vectors, string label)
    {
        StringBuilder b = new();
        b.Append(label);
        b.Append(" ");
        b.AppendJoin(", ", vectors);
        Debug.Log(b);
    }
}

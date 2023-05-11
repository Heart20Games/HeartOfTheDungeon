using ScriptableObjectDropdown;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CSContext", menuName = "Context Steering/Context", order = 1)]
public class CSContext : ScriptableObject
{
    public enum ContextType { Peer, Target, Obstacle, None }

    public Contexts contexts = defaultContexts;

    static readonly public Contexts defaultContexts = new(
        new(ContextType.Peer, 1f, 0f, 5f, -1f, 50f),
        new(ContextType.Target, 1f, 0f, 1000f, -1f, 50f),
        new(ContextType.Obstacle, 1f, 0f, 5f, -1f, 50f)
    );

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
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Body.Behavior.ContextSteering
{
    using static CSIdentity;

    [CreateAssetMenu(fileName = "CSContext", menuName = "Context Steering/Context", order = 1)]
    public class CSContext : ScriptableObject
    {
        public enum ContextType { Peer, Target, Obstacle, None }

        public Context[] contexts = new Context[]
        {
            new(Identity.Friend, 1f, 0f, 5f, -1f, 50f),
            new(Identity.Foe, 1f, 0f, 5f, -1f, 50f),
            new(Identity.Target, 1f, 0f, 1000f, -1f, 50f),
            new(Identity.Obstacle, 1f, 0f, 5f, -1f, 50f),
        };
        public static Context fallbackContext = new(Identity.Neutral, 1f, 0f, 5f, -1f, 50f);
        
        public Dictionary<Identity, Context> contextMap = null;
        public Dictionary<Identity, Context> ContextMap
        {
            get
            {
                if (contextMap == null)  
                {
                    contextMap = new();
                    for(int i = 0; i < contexts.Length; i++)
                    {
                        Context context = contexts[i];
                        contextMap[context.identity] = context;
                    }
                }
                return contextMap;
            }
        }
        public Context this[Identity identity]
        {
            get
            {
                if (ContextMap.TryGetValue(identity, out Context context))
                {
                    return context;
                }
                return fallbackContext;
            }
        }

        //public Contexts contexts = defaultContexts;

        //static readonly public Contexts defaultContexts = new(
        //    new(ContextType.Peer, 1f, 0f, 5f, -1f, 50f),
        //    new(ContextType.Target, 1f, 0f, 1000f, -1f, 50f),
        //    new(ContextType.Obstacle, 1f, 0f, 5f, -1f, 50f)
        //);

        //[Serializable]
        //public struct Contexts
        //{
        //    public Contexts(Context peer, Context target, Context obstacle)
        //    {
        //        this.peer = peer;
        //        this.target = target;
        //        this.obstacle = obstacle;
        //    }
        //    public Context peer;
        //    public Context target;
        //    public Context obstacle;
        //    public Context GetContext(ContextType type)
        //    {
        //        switch (type)
        //        {
        //            case ContextType.Peer: return peer;
        //            case ContextType.Target: return target;
        //            case ContextType.Obstacle: return obstacle;
        //            default: return new(ContextType.None, 0f, 0f, 0f, -1f, 0f);
        //        }
        //    }
        //}

        [Serializable]
        public struct Context
        {
            public Context(Identity identity, float weight, float minDistance, float maxDistance, float cullDistance, float falloff)
            {
                name = identity.ToString();
                this.identity = identity;
                this.weight = weight;
                this.minDistance = minDistance;
                this.maxDistance = maxDistance;
                this.cullDistance = cullDistance >= 0f ? cullDistance : float.MaxValue;
                this.falloff = falloff;
            }
            public string name;
            public Identity identity;
            public float weight;
            public float minDistance;
            public float maxDistance;
            public float cullDistance;
            public float falloff;
        }
    }
}
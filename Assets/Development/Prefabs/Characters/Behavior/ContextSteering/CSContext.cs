using System;
using System.Collections.Generic;
using UnityEngine;

namespace Body.Behavior.ContextSteering
{
    using static CSIdentity;

    [CreateAssetMenu(fileName = "CSContext", menuName = "Context Steering/Context", order = 1)]
    public class CSContext : ScriptableObject
    {
        //public enum ContextType { Peer, Target, Obstacle, None }
        public enum Range { Target, Close, Far, InRange, InAttackRange, None }

        public Context[] contexts = new Context[]
        {
            defaultContext
        };
        public List<Context> test = new();
        public static readonly Identity defaultIdentity = Identity.Neutral;
        public static readonly Range defaultRange = Range.None;
        public static readonly Vector2 defaultWeight = new(1f, 0f);
        public static readonly Vector2 defaultGradient = new(0f, 5f);
        public static readonly Vector2 defaultDeadzone = new(0f, -1f);
        public static readonly float defaultFallof = 50f;
        public static readonly Context defaultContext = new(defaultIdentity, defaultRange, defaultWeight, defaultGradient, defaultDeadzone, defaultFallof);
        public static Context DefaultContext(Identity identity)
        {
            return new(identity, defaultRange, defaultWeight, defaultGradient, defaultDeadzone, defaultFallof);
        }

        public Dictionary<Identity, List<Context>> contextMap = null;
        public Dictionary<Identity, List<Context>> ContextMap { get { return contextMap ?? Initialize(); } }

        public ContextGenerator generator = new();


        public void Awake()
        {
            Initialize();
        }

        public Dictionary<Identity, List<Context>> Initialize()
        {
            contextMap ??= new();
            contextMap.Clear();
            for (int i = 0; i < contexts.Length; i++)
            {
                Context context = contexts[i];
                bool found = contextMap.TryGetValue(context.identity, out List<Context> values);
                if (!found)
                {
                    values = new();
                    contextMap.Add(context.identity, values);
                }
                values.Add(context);
            }
            generator.GenerateContexts(ref contextMap);
            test.Clear();
            generator.GenerateContexts(test);
            return ContextMap;
        }

        public List<Context> this[Identity identity]
        {
            get
            {
                return ContextMap[identity];
            }
        }

        public bool TryGet(Identity identity, out List<Context> result)
        {
            return ContextMap.TryGetValue(identity, out result);
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
        public struct ContextVector
        {
            public ContextVector(Vector2 weight, Vector2 gradient, Vector2 deadzone, float falloff)
            {
                this.weight = weight;
                this.gradient = gradient;
                this.deadzone = new Vector2(Mathf.Repeat(deadzone.x, float.MaxValue), Mathf.Repeat(deadzone.y, float.MaxValue));
                this.falloff = falloff;
            }
            // All vectors are one-dimensional, but defined by two points.
            // The weight vector is a bi-directional vector along the h-axis indicating the weight corresponding to the high and low ends of the gradient vector.
            public Vector2 weight;
            // The gradient vector indicates the start and end of the weight gradient, with final weight values above and below the vector being constant.
            public Vector2 gradient;
            // The deadzone vector indicates the cutoff range beyond which all final weights are equal to zero.
            public Vector2 deadzone;
            // The falloff is the angle from the y-axis at which the final weight drops to zero.
            public float falloff;
        }

        [Serializable]
        public struct Context
        {
            public Context(Identity identity, Range range, Vector2 weight, Vector2 gradient, Vector2 deadzone, float falloff)
            {
                name = identity.ToString() + range.ToString();
                this.identity = identity;
                this.range = range;
                this.vector = new(weight, gradient, deadzone, falloff);
            }
            public string name;
            public Identity identity;
            public Range range;
            public ContextVector vector;
        }
    }
}
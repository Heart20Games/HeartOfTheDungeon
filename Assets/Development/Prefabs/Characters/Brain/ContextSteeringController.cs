using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class ContextSteeringController : MonoBehaviour
{
    [Serializable]
    public struct Context
    {
        public Context(float weight, float minDistance, float maxDistance, float falloff)
        {
            this.weight = weight;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.falloff = falloff;
        }

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
            name = identity.HumanName();
            this.identity = identity;
            this.mapType = mapType;
        }

        public string name;
        public Identity identity;
        public MapType mapType;
    }


    [HideInInspector] static public Vector2[] baseline;
    [HideInInspector] public float[] interest;
    [HideInInspector] public float[] danger;

    public enum ContextType { Peer, Target, Obstacle }
    public enum MapType { Interest, Danger, None }
    private Context[] contexts;
    private float[][] maps;

    public int resolution = 12;
    public Context peerContext = new(1f, 0f, 5f, 20f);
    public Context targetContext = new(1f, 0f, 1000f, 20f);
    public Context obstacleContext = new(1f, 0f, 5f, 20f);

    public enum Identity { Neutral, Friend, Foe }
    public Identity identity = Identity.Neutral;
    private Dictionary<Identity, MapType> identityMap = new();
    public List<IdentityMapPair> pairs = new(new IdentityMapPair[] 
    {
        new(Identity.Neutral, MapType.None),
        new IdentityMapPair(Identity.Foe, MapType.Interest),
        new IdentityMapPair(Identity.Friend, MapType.Danger)
    });

    public float testSpeed = 3f;
    public bool active = false;

    private readonly List<Transform> obstacles = new();
    private new Rigidbody rigidbody;
    private bool initialized = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        Initialize();
    }

    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        // Baseline
        if (baseline == null)
        {
            baseline = new Vector2[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float angle = Mathf.Lerp(0, 360, i / resolution);
                baseline[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }
        }

        // Contexts
        contexts = new Context[] { peerContext, targetContext, obstacleContext };

        // Maps
        interest = new float[resolution];
        danger = new float[resolution];
        maps = new float[][] { interest, danger };

        // Identities
        foreach (var pair in pairs)
        {
            identityMap[pair.identity] = pair.mapType;
        }
    }

    private void FixedUpdate()
    {
        if (active)
        {
            Vector2 vector = GetVector();
            rigidbody.velocity = testSpeed * Time.fixedDeltaTime * new Vector3(vector.x, 0, vector.y);
        }
    }

    public Vector2 GetVector()
    {
        Vector2 vector = new();
        for (int i = 0; i < resolution; i++)
        {
            vector += baseline[i] * interest[i];
            vector -= baseline[i] * danger[i];
            interest[i] = 0f;
            danger[i] = 0f;
        }
        return vector.normalized;
    }

    public MapType GetMapOf(Identity id)
    {
        return identityMap[id];
    }

    public void MapTo(Vector2 vector, MapType mapType, ContextType contextType)
    {
        // Map / Context
        float[] map = maps[(int)mapType];
        Context context = contexts[(int)contextType];

        // Weight
        float distance = vector.magnitude - context.minDistance;
        float range = (context.maxDistance - context.minDistance);
        float weight = Mathf.Lerp(context.weight, 0f, distance / range);

        if (weight > 0f)
        {
            // Angle / Slot
            float angle = vector.x != 0f ? Mathf.Atan(vector.y / vector.x) : Mathf.Sign(vector.y) * 90;
            float slot = (angle / 360) * resolution;

            Assert.IsFalse(float.IsNaN(angle));
            Assert.IsFalse(float.IsNaN(slot));

            // Falloff
            float falloff = (context.falloff / 360) * resolution;
            float falloffHigh = slot + falloff;
            float falloffLow = slot - falloff;

            // Slots
            int nextSlot = Mathf.CeilToInt(slot);
            int prevSlot = Mathf.FloorToInt(slot);
            int highSlot = Mathf.FloorToInt(falloffHigh);
            int lowSlot = Mathf.CeilToInt(falloffLow);

            // Add it up
            for (int i = nextSlot; i <= highSlot; i++)
            {
                map[i] += Mathf.Lerp(weight, 0, (i - slot) / falloff);
            }
            for (int i = prevSlot; i >= lowSlot; i--)
            {
                map[i] += Mathf.Lerp(weight, 0, (slot - i) / falloff);
            }
        }
    }

    public void AddObstacle(Impact impact)
    {
        if (!impact.other.TryGetComponent<ContextSteeringController>(out _))
        {
            if (!obstacles.Contains(impact.other.transform))
            {
                obstacles.Add(impact.other.transform);
            }
        }
    }

    public void RemoveObstacle(Impact impact)
    {
        if (!impact.other.TryGetComponent<ContextSteeringController>(out _))
        {
            obstacles.Remove(impact.other.transform);
        }
    }
}

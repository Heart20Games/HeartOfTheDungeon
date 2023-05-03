using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

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
    public List<Identity> interests = new();
    public List<Identity> dangers = new();
    private bool[] bInterests;
    private bool[] bDangers;
    private bool[][] identities;

    public float testSpeed = 3f;

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
        bInterests = new bool[Enum.GetValues(typeof(Identity)).Length];
        bDangers = new bool[Enum.GetValues(typeof(Identity)).Length];
        identities = new bool[][] { bInterests, bDangers };
        foreach (Identity id in Enum.GetValues(typeof(Identity)))
        {
            bInterests[(int)id] = interests.Contains(id);
            bDangers[(int)id] = dangers.Contains(id);
        }
    }

    private void FixedUpdate()
    {
        Vector2 vector = GetVector();
        rigidbody.velocity = testSpeed * Time.fixedDeltaTime * new Vector3(vector.x, 0, vector.y);
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
        if (MappedTo(id, MapType.Interest))
        {
            return MapType.Interest;
        }
        else if (MappedTo(id, MapType.Danger))
        {
            return MapType.Danger;
        }
        else
        {
            return MapType.None;
        }
    }

    public bool MappedTo(Identity id, MapType map)
    {
        return identities[(int)map][(int)id];
    }

    public void MapTo(Vector2 vector, MapType mapType, ContextType contextType)
    {
        // Map / Context
        float[] map = maps[(int)mapType];
        Context context = contexts[(int)contextType];

        // Angle / Slot
        float angle = Mathf.Atan(vector.y / vector.x);
        float slot = (angle / 360) * resolution;

        // Weight
        float distance = vector.magnitude - context.minDistance;
        float range = (context.maxDistance - context.minDistance);
        float weight = Mathf.Lerp(context.weight, 0f, distance / range);

        if (weight > 0f)
        {
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

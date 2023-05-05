using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Assertions;
using static ContextSteeringStructs;

[RequireComponent(typeof(Rigidbody))]
public class ContextSteeringController : MonoBehaviour
{
    public float testSpeed = 3f;
    public bool draw;
    [SerializeField] private bool active = false;
    public bool Active
    {
        get => active;
        set => SetActive(value);
    }

    // Destination
    [SerializeField] private Vector3 destination = new();
    public bool following = false;
    public Vector3 Destination { get => destination; set { destination = value; following = true; } }

    // Readonly
    private readonly Maps maps = new(new float[resolution], new float[resolution]);
    private readonly Dictionary<Identity, MapType> identityMap = new();
    private readonly List<Transform> obstacles = new();

    // Identity
    [SerializeField] private Contexts contexts = defaultContexts;
    public Identity identity = Identity.Neutral;
    public IdentityMapPair[] pairs = defaultPairs;

    // Initialization
    private new Rigidbody rigidbody;
    private bool initialized = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Active = Active;
        Initialize();
    }

    public void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            // Identity Map
            foreach (var pair in pairs)
            {
                identityMap[pair.identity] = pair.mapType;
            }
        }
    }

    public void SetActive(bool active)
    {
        this.active = active;
        if (!active && rigidbody != null)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (active)
        {
            MapTo(transform.position - destination, MapType.Interest, ContextType.Target);
            Draw();
            Vector2 vector = GetVector();
            rigidbody.velocity = testSpeed * Time.fixedDeltaTime * new Vector3(vector.x, 0, vector.y);
        }
    }

    // Get
    public Vector2 GetVector()
    {
        Vector2 vector = new();
        float[] interests = maps.GetMap(MapType.Interest);
        float[] dangers = maps.GetMap(MapType.Danger);
        for (int i = 0; i < resolution; i++)
        {
            vector += Baseline[i] * interests[i];
            vector -= Baseline[i] * dangers[i];
            interests[i] = 0f;
            dangers[i] = 0f;
        }
        return vector.normalized;
    }

    public MapType GetMapOf(Identity id)
    {
        return identityMap[id];
    }

    // Set
    public void MapTo(Vector2 vector, MapType mapType, ContextType contextType)
    {
        if (vector != Vector2.zero)
        {
            // Map / Context
            float[] map = maps.GetMap(mapType);
            Context context = contexts.GetContext(contextType);

            // Weight
            float distance = vector.magnitude - context.minDistance;
            float range = (context.maxDistance - context.minDistance);
            float weight = Mathf.Lerp(context.weight, 0f, distance / range);

            if (weight > 0f)
            {
                // Angle / Slot
                float angle = vector.x != 0f ? Mathf.Rad2Deg * Mathf.Atan(vector.y / vector.x) : Mathf.Sign(vector.y) * 90;
                float slot = (angle / 360) * resolution;

                if (Mathf.Round(slot) != 0)
                {
                    Debug.Log("Not Zero: " + Mathf.Round(slot));
                    print(vector);
                }
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
    }

    // Obstacles
    public void AddObstacle(Impact impact)
    {
        if (!impact.other.TryGetComponent<ContextSteeringController>(out _))
        {
            if (!obstacles.Contains(impact.other.transform))
            {
                Transform other = impact.other.transform;
                obstacles.Add(other);
                Vector2 otherPos = new(other.position.x, other.position.z);
                Vector2 curPos = new(transform.position.x, transform.position.z);
                MapTo(otherPos - curPos, MapType.Danger, ContextType.Obstacle);
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

    // Draw
    public void Draw()
    {
        if (draw)
        {
            float[] interests = maps.GetMap(MapType.Interest);
            float[] dangers = maps.GetMap(MapType.Danger);
            for (int i = 0; i < resolution; i++)
            {
                if (interests[i] > 0)
                {
                    Debug.DrawRay(transform.position, 100 * interests[i] * Baseline[i], Color.green, Time.deltaTime);
                }
                if (dangers[i] > 0)
                {
                    Debug.DrawRay(transform.position, -100 * dangers[i] * Baseline[i], Color.red, Time.deltaTime);
                }
            }
        }
    }
}

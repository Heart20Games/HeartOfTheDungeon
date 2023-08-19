using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using ScriptableObjectDropdown;
using UnityEngine.Events;

namespace Body.Behavior.ContextSteering
{
    using static CSContext;
    using static CSMapping;
    using static CSIdentity;
    using static Brain;

    [RequireComponent(typeof(Rigidbody))]
    public class CSController : BaseMonoBehaviour
    {
        // Presets
        [Header("Preset")]
        [ScriptableObjectDropdown(typeof(CSPreset), grouping = ScriptableObjectGrouping.ByFolderFlat)]
        public ScriptableObjectReference preset;
        public CSPreset Preset { get { return (CSPreset)preset.value; } set { preset.value = value; } }

        // Context
        private readonly FullContext context = new();
        public FullContext Context { get { return context.initialized ? context : context.Initialize(Preset); } }

        // Scaling
        public float scale = 0f;
        public float Scale { get { return scale == 0 ? (Preset.scale == 0 ? 1f : Preset.scale) : scale; } }
        public float DrawScale => Preset.drawScale;
        public bool DrawRays => Preset.draw;

        // Identity
        public Identity identity = Identity.Neutral;

        // Active
        [Header("State")]
        [SerializeField] private bool active = false;
        public bool Active
        { 
            get => active;
            set
            {
                active = value;
                if (!active && rigidbody != null)
                    rigidbody.velocity = Vector3.zero;
            }
        }
        [SerializeField] private bool alive = true;
        public bool Alive
        {
            get => alive;
            set
            {
                alive = value;
                if (!value) Active = false;
            }
        }

        // Destination
        [Header("Destination")]
        [SerializeField] private Vector3 destinationStep = new();
        [SerializeField] private float destinationDistance = 0f;
        public float destinationScale = 1f;
        public void SetDestination(Vector3 step, float distance)
        {
            destinationStep = step;
            destinationDistance = distance;
        }

        // Debug
        [Header("Debugging")]
        public bool debug = false;
        private readonly float ResultRadius = 0.5f;
        private readonly float CircleRadius = 0.55f;
        private readonly float SourceRadius = 0.15f;
        private readonly float ActualRadius = 0.05f;
        
        // Vector
        [Header("Movement")]
        [SerializeField] private Vector2 currentVector;
        public Vector2 CurrentVector { get => currentVector; set { currentVector = value; onSetVector.Invoke(currentVector); } }
        public float Speed => Preset.testSpeed;
        public bool moveSelf = true;
        private Rigidbody rigidbody;
        public UnityEvent<Vector2> onSetVector;

        [Header("Noise")]
        public MovementNoise noise;
        public bool useNoise = false;
        private float noiseOffset;

        // Generated
        [Header("Context Data")]
        public List<Context> activeContexts = new();
        private readonly List<Transform> obstacles = new();
        private Maps Maps { get; } = new(null, null);
        public UnityEvent onFoeContextActive;


        // Initialize
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            context.debug = debug;
            Active = Active;
            Alive = Alive;
            noiseOffset = Random.value * 1000;
        }

        // Active Contexts
        public bool HasActiveContext(Identity identity, Range range)
        {
            for(int i = 0; i < activeContexts.Count; i++)
            {
                Context context = activeContexts[i];
                if (debug && (range == Range.InAttackRange && identity == Identity.Foe) && (context.identity == identity || context.range == range))
                {
                    print($"Context ({identity}, {range}) -> ({context.identity}, {context.range})");
                }
                if (context.identity == identity && context.range == range)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasActiveContext(Context context)
        {
            return activeContexts.Contains(context);
        }

        // Update
        private void FixedUpdate()
        {
            if (active)
            {
                if (destinationDistance > 0)
                {
                    Vector2 destinationVector = (destinationStep - transform.position).XZVector();
                    MapTo(destinationVector, Identity.Target, destinationDistance * destinationScale);
                }
                DrawMaps();
                Vector3 vector = GetVector();
                if (moveSelf)
                {
                    if (debug)
                    {
                        if (vector.magnitude != 0 && Speed != 0) print($"{gameObject.name} is moving.");
                        else if (vector.magnitude != Speed) print($"{gameObject.name} not moving. (vector:{vector.magnitude}, speed:{Speed})");
                    }
                    rigidbody.velocity = Speed * Scale * Time.fixedDeltaTime * vector;
                }
                CurrentVector = vector.XZVector();
                onSetVector.Invoke( CurrentVector );
            }
        }

        // Vector

        public Vector3 GetVector()
        {
            Vector3 vector = new();
            Maps.CalculateVectors();
            Dictionary<Identity, Vector3> vectors = Maps.vectors;

            foreach (Identity identity in vectors.Keys)
                vector += vectors[identity];

            if (vectors.Count > 0)
                vector /= vectors.Count;

            if (useNoise && noise != null)
            {
                noise.ApplyNoise(ref vector, noiseOffset);
            }

            Assert.IsFalse(float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z));

            return vector;
        }


        // Mappable

        private readonly List<Context> mappableContexts = new();
        public List<Context> IsIdentityMappable(float distance, Identity identity)
        {
            mappableContexts.Clear();
            if (Context != null && distance > 0)
            {
                // Map Context
                if (Context.TryGet(identity, out List<Context> contexts))
                {
                    for (int i = 0; i < contexts.Count; i++)
                    {
                        if (IsContextMappable(distance, contexts[i]))
                            mappableContexts.Add(contexts[i]);
                    }
                }
            }
            return mappableContexts;
        }

        public bool IsContextMappable(float distance, Context context)
        {
            ContextVector cVector = context.vector;
            return distance == Mathf.Clamp(distance, cVector.deadzone.x, cVector.deadzone.y);
        }


        // Map To

        public void MapTo(Vector2 vector, Identity identity, float distanceOverride=-1)
        {
            if (Context != null && vector != Vector2.zero)
            {
                Vector3 alt = new(vector.x, 0f, vector.y);
                DrawPart(NA, alt.normalized, CircleRadius+SourceRadius, CircleRadius+SourceRadius+ActualRadius);

                // Map Context
                if (Context.TryGet(identity, out List<Context> contexts))
                {
                    MapContexts(vector, contexts, distanceOverride);
                }
            }
        }

        public void MapContexts(Vector2 vector, List<Context> contexts, float distanceOverride=-1)
        {
            for (int i = 0; i < contexts.Count; i++)
            {
                MapContext(vector, contexts[i], distanceOverride);
            }
        }

        public void MapContext(Vector2 vector, Context context, float distanceOverride=-1)
        {
            vector /= Scale;
            ContextVector cVector = context.vector;
            float magnitude = distanceOverride >= 0 ? distanceOverride : vector.magnitude;
            if (magnitude == Mathf.Clamp(magnitude, cVector.deadzone.x, cVector.deadzone.y))
            {
                if (debug && context.range == Range.InAttackRange) print("Caster Range!");
                if (!activeContexts.Contains(context))
                {
                    activeContexts.Add(context);
                    if (context.identity == Identity.Foe)
                        onFoeContextActive.Invoke();
                }

                // Distance and Range
                if (debug) print("Calculating distance.");
                float distance = Mathf.Min(magnitude - cVector.gradient.x, cVector.gradient.y);
                float range = (cVector.gradient.y - cVector.gradient.x);

                float t = 0.5f;
                if (range != 0)
                {
                    t = distance / range;
                    Assert.IsFalse(float.IsNaN(t));
                }

                if (debug) print("Calculating weight");

                // Weight
                float weight = Mathf.Lerp(cVector.weight.x, cVector.weight.y, t);
                Assert.IsFalse(float.IsNaN(weight));
                if (weight != 0f)
                {
                    if (debug) print("Calculating falloff");

                    // Map w/ Falloff
                    float falloff = (cVector.falloff / 360) * resolution;
                    MapType type = weight < 0 ? MapType.Danger : MapType.Interest;
                    MapToSlots(weight, vector, falloff, Maps[type, context.identity]);
                }
            }
        }

        private void MapToSlots(float weight, Vector2 vector, float falloff, Map map)
        {
            weight *= map.sign;

            // Angle / Slot
            float angle = -Mathf.Rad2Deg * Mathf.Acos(vector.x / vector.magnitude);
            angle = Mathf.Repeat((vector.y > 0f ? 360 - angle : angle), 360);
            float slot = Mathf.Repeat((angle / 360) * resolution, resolution - 1);

            // Debugging and Assertions
            DrawPart(map.sign, Baseline[Mathf.RoundToInt(slot)], CircleRadius, CircleRadius + SourceRadius);
            Assert.IsFalse(float.IsNaN(angle));
            Assert.IsFalse(float.IsNaN(slot));
            AssertInRange(angle, 0, 360);
            AssertInRange(slot, 0, resolution - 1);

            // Falloff
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
                int ii = (int)Mathf.Repeat(i, resolution - 1);
                map[ii] += Mathf.Lerp(weight, 0, (i - slot) / falloff);
                Assert.IsFalse(float.IsNaN(map[ii]));
            }
            for (int i = prevSlot; i >= lowSlot; i--)
            {
                int ii = (int)Mathf.Repeat(i, resolution - 1);
                map[ii] += Mathf.Lerp(weight, 0, (slot - i) / falloff);
                Assert.IsFalse(float.IsNaN(map[ii]));
            }

            // Up the component count
            int mcc = map.componentCount;
            map.IncrementComponentCount();
        }


        // Obstacles

        public void AddObstacle(Impact impact)
        {
            if (!impact.other.TryGetComponent<CSController>(out _))
            {
                if (!obstacles.Contains(impact.other.transform))
                {
                    Transform other = impact.other.transform;
                    obstacles.Add(other);
                    Vector2 otherPos = new(other.position.x, other.position.z);
                    Vector2 curPos = new(transform.position.x, transform.position.z);
                    MapTo(otherPos - curPos, Identity.Obstacle);
                }
            }
        }

        public void RemoveObstacle(Impact impact)
        {
            if (!impact.other.TryGetComponent<CSController>(out _))
            {
                obstacles.Remove(impact.other.transform);
            }
        }


        // Assertion

        private void AssertInRange(float value, float min, float max)
        {
            Assert.IsTrue(value >= min);
            Assert.IsTrue(value <= max);
        }


        // Draw
        public void DrawMaps()
        {
            if (DrawRays)
            {
                float maxValue = Maps.MaxValue();
                for (int i = 0; i < Maps.Length; i++)
                {
                    if (Maps[i].Count > 0)
                    {
                        for (int j = 0; j < resolution; j++)
                        {
                            float total = Maps[i, j];
                            if (total > 0)
                            {
                                //Assert.IsTrue(total <= maxValue);
                                DrawPart(Maps.Sign(i), Baseline[j], 0f, ResultRadius, Mathf.Lerp(0f, ResultRadius, total / maxValue));
                            }
                        }
                    }
                }
                DrawCircle(CircleRadius);
            }
        }

        public void DrawCircle(float radius)
        {
            if (DrawRays)
            {
                for (int i = 0; i < resolution; i++)
                {
                    Vector3 start = transform.position + (DrawScale * radius * Baseline[i]);
                    Vector3 end = transform.position + (DrawScale * radius * Baseline[(i + 1) % resolution]);
                    Debug.DrawLine(start, end, Color.white, Time.fixedDeltaTime * 2);
                }
            }
        }

        public void DrawPart(sbyte sign, Vector3 dir, float buffer=0f, float limit=float.MaxValue, float magnitude = 1f, float duration=1f)
        {
            if (DrawRays)
            {
                Color color = sign switch
                {
                    POS => Color.green,
                    NEG => Color.red,
                    NA => Color.cyan,
                    _ => Color.yellow,
                };
                //print("Color: " + ColorString(color));
                sign = sign == 0 ? (sbyte)1 : sign;
                sign = (Mathf.Abs(sign) > 1) ? (sbyte)(1 * Mathf.Sign(sign)) : sign;
                Vector3 direction = DrawScale * sign * dir;
                float mag = Mathf.Min(magnitude, limit-buffer);
                Vector3 vector = mag * direction;
                Vector3 start = transform.position + (buffer * direction);
                Debug.DrawRay(start, vector, color, Time.fixedDeltaTime * duration);
            }
        }

        private string ColorString(Color color)
        {
            return color == Color.blue ?
                    "Blue" : 
                color == Color.green ? 
                    "Green" :
                color == Color.red ? 
                    "Red" :
                color == Color.cyan ?
                    "Cyan" :
                color == Color.yellow ?
                    "Yellow" :
                color.ToString();
        }
    }
}

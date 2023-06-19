using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using ScriptableObjectDropdown;

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
        [ScriptableObjectDropdown(typeof(CSPreset), grouping = ScriptableObjectGrouping.ByFolderFlat)]
        public ScriptableObjectReference preset;
        public CSPreset Preset { get { return (CSPreset)preset.value; } set { preset.value = value; } }

        // Context
        private readonly FullContext context = new();
        public FullContext Context { get { return context.initialized ? context : context.Initialize(Preset); } }

        // Parts
        public float scale = 0f;
        public float Scale { get { return scale == 0 ? (Preset.scale == 0 ? 1f : Preset.scale) : scale; } }
        public float Speed => Preset.testSpeed;
        public float DrawScale => Preset.drawScale;
        public bool DrawRays => Preset.draw;
        public Identity Identity { get => Preset.Identity; }

        // Initialization
        private Rigidbody rigidbody;

        // Active
        [SerializeField] private bool active = false;
        public bool Active { get => active; set => SetActive(value); }

        // Destination
        [SerializeField] private Vector3 destination = new();
        public bool following = false;
        public Vector3 Destination { get => destination; set { destination = value; following = true; } }

        // Vector
        [HideInInspector] public Vector2 currentVector;

        // Generated
        private readonly List<Transform> obstacles = new();
        public readonly List<Context> activeContexts = new();
        private Maps Maps { get; } = new(null, null);

        // Debug
        private readonly float ResultRadius = 0.5f;
        private readonly float CircleRadius = 0.55f;
        private readonly float SourceRadius = 0.15f;
        private readonly float ActualRadius = 0.05f;

        public bool debug = false;

        // Initialize
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            context.debug = debug;
            Active = Active;
        }

        // Activate
        public void SetActive(bool active)
        {
            this.active = active;
            if (!active && rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
            }
        }

        // Active Contexts
        public bool HasActiveContext(Identity identity, Range range)
        {
            for(int i = 0; i < activeContexts.Count; i++)
            {
                Context context = activeContexts[i];
                if (context.identity == identity && context.range == range)
                {
                    return true;
                }
            }
            return false;
        }

        // Update
        private void FixedUpdate()
        {
            if (following)
            {
                MapTo((destination - transform.position).XZVector(), Identity.Target);
            }
            Draw();
            Vector3 vector = GetVector();
            if (active)
            {
                if (debug)
                {
                    if (vector.magnitude != 0 && Speed != 0) print($"{gameObject.name} is moving.");
                    else if (vector.magnitude != Speed) print($"{gameObject.name} not moving. (vector:{vector.magnitude}, speed:{Speed})");
                }
                rigidbody.velocity = Speed * Scale * Time.fixedDeltaTime * vector;
            }
            currentVector = vector.XZVector();
        }

        // Vector

        public Vector3 GetVector()
        {
            Vector3 vector = new();
            Map interests = Maps[MapType.Interest];
            Map dangers = Maps[MapType.Danger];
            int componentCount = 1; // interests.componentCount + dangers.componentCount;
            for (int i = 0; i < resolution; i++)
            {
                Assert.IsFalse(float.IsNaN(interests[i]));
                Assert.IsFalse(float.IsNaN(dangers[i]));
                vector += (interests[i] / componentCount) * interests.sign * Baseline[i];
                vector += (dangers[i] / componentCount) * dangers.sign * Baseline[i];
                if (debug)
                {
                    if (interests[i] != 0 && dangers[i] != 0)
                        print($"Found interest or danger. (interest:{interests[i]}, danger:{dangers[i]}");
                    if (interests.sign == 0 || dangers.sign == 0)
                        Debug.LogWarning("Interests and Dangers should be negative or positive, not zero.");
                    if (Baseline[i].magnitude == 0)
                        Debug.LogWarning("Baseline value should never be zero.");
                    Debug.DrawRay(transform.position, vector*2, Color.white, Time.fixedDeltaTime);
                }
                interests[i] = 0f;
                dangers[i] = 0f;
            }
            interests.componentCount = 0;
            dangers.componentCount = 0;
            return vector.normalized;
        }

        public Identity RelativeIdentity(Identity id)
        {
            bool friendOrFoe = (id == Identity.Friend) || (id == Identity.Foe);
            Identity opponent = (id == Identity ? Identity.Friend : Identity.Foe);
            return (!friendOrFoe) ? id : opponent;
        }

        // Set
        public void MapTo(Vector2 vector, Identity identity)
        {
            if (Context != null && vector != Vector2.zero)
            {
                Vector3 alt = new(vector.x, 0f, vector.y);
                DrawPart(NA, alt.normalized, CircleRadius+SourceRadius, CircleRadius+SourceRadius+ActualRadius);

                // Map Context
                if (Context.TryGet(identity, out List<Context> contexts))
                {
                    for (int i = 0; i < contexts.Count; i++)
                    {
                        MapContext(vector, contexts[i]);
                    }
                }
            }
        }

        public void MapContext(Vector2 vector, Context context)
        {
            vector /= Scale;
            ContextVector cVector = context.vector;
            if (vector.magnitude == Mathf.Clamp(vector.magnitude, cVector.deadzone.x, cVector.deadzone.y))
            {
                if (!activeContexts.Contains(context))
                {
                    activeContexts.Add(context);
                }

                // Distance and Range
                float distance = Mathf.Min(vector.magnitude - cVector.gradient.x, cVector.gradient.y);
                float range = (cVector.gradient.y - cVector.gradient.x);
                if (range != 0)
                {
                    float t = distance / range;
                    Assert.IsFalse(float.IsNaN(t));
                    
                    // Weight
                    float weight = Mathf.Lerp(cVector.weight.x, cVector.weight.y, t);
                    Assert.IsFalse(float.IsNaN(weight));
                    if (weight != 0f)
                    {
                        // Map w/ Falloff
                        float falloff = (cVector.falloff / 360) * resolution;
                        MapToSlots(weight, vector, falloff);
                    }
                }
            }
        }

        private void MapToSlots(float weight, Vector2 vector, float falloff)
        {
            Map map = weight < 0 ? Maps.dangers : Maps.interests;
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
            map.componentCount += 1;
        }

        private void AssertInRange(float value, float min, float max)
        {
            Assert.IsTrue(value >= min);
            Assert.IsTrue(value <= max);
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

        // Draw
        public void Draw()
        {
            if (DrawRays)
            {
                float maxValue = Maps.MaxValue();
                for (int i = 0; i < Maps.Length; i++)
                {
                    Map map = Maps[i];
                    if (!map.IsZero())
                    {
                        //print("Draw: " + (map.sign < 0 ? "Danger" : "Interest"));
                        for (int j = 0; j < resolution; j++)
                        {
                            if (map[j] > 0)
                            {
                                Assert.IsTrue(map[j] <= maxValue);
                                DrawPart(map.sign, Baseline[j], 0f, ResultRadius, Mathf.Lerp(0f, ResultRadius, map[j] / maxValue));
                            }
                        }
                    }
                }
                DrawCircle(CircleRadius);
            }
        }

        public void DrawCircle(float radius)
        {
            for (int i = 0; i < resolution; i++)
            {
                Vector3 start = transform.position + (DrawScale * radius * Baseline[i]);
                Vector3 end = transform.position + (DrawScale * radius * Baseline[(i + 1) % resolution]);
                Debug.DrawLine(start, end, Color.white, Time.fixedDeltaTime * 2);
            }
        }

        public void DrawPart(sbyte sign, Vector3 dir, float buffer=0f, float limit=float.MaxValue, float magnitude = 1f, float duration=1f)
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

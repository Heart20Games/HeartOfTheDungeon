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
        private CSContext context;
        public CSContext Context
        {
            get
            {
                if (context == null) context = Preset.GetContext(Action.Chase);
                return context;
            }
            set => context = value;
        }

        // Idenity
        private CSIdentity relationships;
        public CSIdentity Relationships
        {
            get
            {
                if (relationships == null) relationships = Preset.GetRelationships(Action.Chase);
                return relationships;
            }
            set => relationships = value;
        }

        // Parts
        public float Speed => Preset.testSpeed;
        public float DrawScale => Preset.drawScale;
        public bool DrawRays => Preset.draw;
        public Identity Identity { get => Preset.Identity; }
        public IdentityMapPair[] Pairs { get => Relationships.pairs; }
        public Dictionary<Identity, IdentityMapPair> IdentityMap { get => Relationships.IdentityMap; }

        // Initialization
        private new Rigidbody rigidbody;

        // Active
        [SerializeField] private bool active = false;
        public bool Active { get => active; set => SetActive(value); }

        // Destination
        [SerializeField] private Vector3 destination = new();
        public bool following = false;
        public Vector3 Destination { get => destination; set { destination = value; following = true; } }

        // Generated
        private readonly List<Transform> obstacles = new();
        private Maps Maps { get; } = new(null, null);

        // Initialize
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
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

        // Update
        private void FixedUpdate()
        {
            if (following)
            {
                MapTo(transform.position - destination, Identity.Target, Maps.interests);
            }
            Draw();
            Vector3 vector = GetVector();
            if (active)
            {
                rigidbody.velocity = Speed * Time.fixedDeltaTime * vector;
            }
        }

        // Vector
        public Vector3 GetVector()
        {
            Vector3 vector = new();
            Map interests = Maps[MapType.Interest];
            Map dangers = Maps[MapType.Danger];
            for (int i = 0; i < resolution; i++)
            {
                vector += interests[i] * interests.sign * Baseline[i];
                vector += dangers[i] * dangers.sign * Baseline[i];
                interests[i] = 0f;
                dangers[i] = 0f;
            }
            return vector.normalized;
        }

        // Mapping
        public Map GetMapOf(Identity id)
        {
            return Relationships == null ? Maps[MapType.None] : Maps[IdentityMap[id].mapType];
        }

        public Identity RelativeIdentity(Identity id)
        {
            bool friendOrFoe = (id == Identity.Friend) || (id == Identity.Foe);
            Identity opponent = (id == Identity ? Identity.Friend : Identity.Foe);
            return (!friendOrFoe) ? id : opponent;
        }

        // Set
        public void MapTo(Vector2 vector, Identity identity, Map map)
        {
            if (Context != null && Relationships != null && vector != Vector2.zero)
            {
                Vector3 alt = new(vector.x, 0f, vector.y);
                DrawPart(map.sign, 0.5f, alt.normalized, 0.5f, 1.0f);

                // Map / Context
                Context context = Context[identity];

                if (vector.magnitude < context.cullDistance)
                {
                    // Weight
                    float idWeight = IdentityMap.TryGetValue(identity, out IdentityMapPair pair) ? pair.weight : 1;
                    float distance = Mathf.Min(vector.magnitude - context.minDistance, context.minDistance);
                    float range = (context.maxDistance - context.minDistance);
                    float weight = Mathf.Lerp(context.weight, 0f, distance / range) * idWeight;

                    if (weight > 0f)
                    {
                        // Angle / Slot
                        float angle = -Mathf.Rad2Deg * Mathf.Acos(vector.x / vector.magnitude);
                        angle = (vector.y > 0f) ? 360 - angle : angle;
                        //float angle = vector.x != 0f ? Mathf.Rad2Deg * Mathf.Atan(vector.y/vector.x) : Mathf.Sign(vector.y) * 90;
                        angle = Mathf.Repeat(angle, 360);
                        float slot = Mathf.Repeat((angle / 360) * resolution, resolution-1);
                        DrawPart(NA, 0.5f, Baseline[Mathf.RoundToInt(slot)], 0.5f, 1f);

                        Assert.IsFalse(float.IsNaN(angle));
                        Assert.IsFalse(float.IsNaN(slot));
                        AssertInRange(angle, 0, 360);
                        AssertInRange(slot, 0, resolution-1);

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
                            int ii = (int)Mathf.Repeat(i, resolution-1);
                            map[ii] += Mathf.Lerp(weight, 0, (i - slot) / falloff);
                        }
                        for (int i = prevSlot; i >= lowSlot; i--)
                        {
                            int ii = (int)Mathf.Repeat(i, resolution-1);
                            map[ii] += Mathf.Lerp(weight, 0, (slot - i) / falloff);
                        }
                    }
                }
            }
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
                    MapTo(otherPos - curPos, Identity.Obstacle, Maps.dangers);
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
                                DrawPart(map.sign, Mathf.Lerp(0f, 0.5f, map[j]/maxValue), Baseline[j], 0f, 0.5f);
                            }
                        }
                    }
                }
                DrawCircle(0.5f);
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

        public void DrawPart(sbyte sign, float magnitude, Vector3 dir, float buffer=0f, float limit=float.MaxValue, float duration=2f)
        {
            Color color = sign switch
            {
                POS => Color.green,
                NEG => Color.red,
                NA => Color.cyan,
                _ => Color.yellow,
            };
            sign = sign == 0 ? (sbyte)1 : sign;
            Vector3 direction = DrawScale * sign * dir;
            Vector3 vector = Mathf.Min(magnitude, limit) * direction;
            Vector3 start = transform.position + (buffer * direction);
            Debug.DrawRay(start, vector, color, Time.fixedDeltaTime * duration);
        }

        private void CheckColor(Color color)
        {
            if (color == Color.blue)
            {
                print("Blue!");
            }
            else if (color == Color.yellow)
            {
                print("Yellow!");
            }
        }
    }
}

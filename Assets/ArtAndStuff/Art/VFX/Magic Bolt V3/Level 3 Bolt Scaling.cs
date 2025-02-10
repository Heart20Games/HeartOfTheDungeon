using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.VFX;

[RequireComponent(typeof(EffectSpawner))]
public class Level3BoltScaling : BaseMonoBehaviour
{
    [Foldout("Configuration", true)]
    [Header("Components")]
    public new Collider collider;
    public VisualEffect bolt;
    [Header("Scale")]
    public float scaleSpeed;
    [SerializeField] private float maxScale = 50f;
    [SerializeField] private bool shouldShootForever;
    [Header("Duration")]
    public bool hasDuration = true;
    [ConditionalField("hasDuration")]
    public float castDuration;
    [Header("Collision")]
    [SerializeField] private float collisionMargin = 0.01f;
    [SerializeField] private float collisionBaseScale = 50f;
    [FormerlySerializedAs("raycastLayer")]
    [SerializeField] private LayerMask scalingLayerMask;
    [SerializeField] private RingCast ringCast;
    private EffectSpawner effectSpawner;
    [Header("Direction")]
    [SerializeField] private bool shouldFollowCrossHair;
    [Header("Debug")]
    [SerializeField] private bool debug = false;
    [SerializeField] private bool debugEnable = false;

    [Foldout("State", true)]
    [Header("State")]
    [ReadOnly][SerializeField] private bool playing = false;
    [ReadOnly][SerializeField] private bool casting = false;
    [SerializeField] private float currentScale;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField][ReadOnly] private int numInLayer;
    [SerializeField][ReadOnly] private int totalNum;
    private Coroutine windDownCoroutine;

    [Foldout("Events", true)]
    [SerializeField] protected UnityEvent onWoundDown = new();

    [Serializable]
    struct RingCast
    {
        public Transform source; // Assumed to share parent w/ target.
        public Transform target; // Assumed to share parent w/ source.
        public float radius;
        public float density;
        public LayerMask layerMask;
        public List<Ray> rays; // This is the list of rays to be checked.
        public List<Vector3> offsets; // This is the output
        public void CalculateRays()
        {
            rays ??= new();
            rays.Clear();
            for (int i = 1; i <= density; i++)
            {
                float arcPos = Mathf.Lerp(0, 2 * Mathf.PI, i / density);
                Vector3 point = radius * new Vector3(Mathf.Cos(arcPos), Mathf.Sin(arcPos), 0);
                Vector3 origin = source.localPosition + point;
                Vector3 destination = target.localPosition + point;
                rays.Add(new(source.TransformPoint(origin), (target.TransformPoint(destination) - source.TransformPoint(origin)).normalized));
            }
        }
        public void Cast(float maxDistance, int? raycastLayer = null)
        {
            offsets ??= new();
            offsets.Clear();
            foreach (var ray in rays)
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 0.1f);
                RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance, raycastLayer ?? layerMask);
                foreach (var hit in hits)
                {
                    offsets.Add(hit.point);
                }
            }
        }
        public void Clear()
        {
            offsets.Clear();
        }
    }

    public bool ShouldFollowCrossHair
    {
        get => shouldFollowCrossHair;
        set => shouldFollowCrossHair = value;
    }

    private void Awake()
    {
        effectSpawner = GetComponent<EffectSpawner>();
    }

    private void Start()
    {
        currentScale = 0f;
    }

    private void OnEnable()
    {
        Print("Enabled.", debugEnable, this);
        if (!playing)
        {
            ChargeUp();
        }
    }

    private void OnDisable()
    {
        Print("Disabled.", debugEnable, this);
        if (casting)
        {
            WindDown();
        }
    }

    public void ChargeUp()
    {
        Print("Charge the laser!", debug);
        bolt.SetFloat("Duration", castDuration);
        bolt.SetBool("Effect End", false);
        bolt.Play();
        collider.gameObject.SetActive(true);
        collider.enabled = true;
        playing = true;
        casting = false;
        effectSpawner.enabled = true;
        currentScale = 0;
        InterruptWindDown();
        UpdateScaling();
    }

    public void InterruptWindDown()
    {
        Print("Interrupt the laser.", debug);
        if (windDownCoroutine != null)
        {
            StopCoroutine(windDownCoroutine);
            windDownCoroutine = null;
        }
    }

    public void WindDownCoroutine()
    {
        InterruptWindDown();
        windDownCoroutine = StartCoroutine(DoWindDown(castDuration));
    }

    public void WindDown()
    {
        Print("Stop the laser!", debug);
        bolt.Stop();
        currentScale = 0f;
        enabled = false;
        playing = false;
        casting = false;
        collider.enabled = false;
        collider.gameObject.SetActive(false);
        bolt.SetBool("Effect End", true);
        windDownCoroutine = null;
        UpdateScaling();
        ringCast.Clear();
        effectSpawner.enabled = false;
        onWoundDown.Invoke();
    }

    private IEnumerator DoWindDown(float duration)
    {
        Print("Cool off the laser...", debug);
        yield return new WaitForSeconds(duration);
        WindDown();
    }

    private void UpdateScaling()
    {
        bolt.SetFloat("Scale", currentScale);
        collider.transform.localScale = new Vector3(collider.transform.localScale.x, collider.transform.localScale.y, (currentScale * collisionBaseScale) + collisionMargin);
    }

    private void UpdateRayCastOffsets()
    {
        ringCast.CalculateRays();
        ringCast.Cast(maxDistance + 1);
        effectSpawner.spawnTargets.Clear();
        effectSpawner.spawnTargets.Add(new(ringCast.target, ringCast.offsets, true));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMaxDistance();

        if (playing)
        {
            Print("Playing...", debug);
            if (!casting)
            {
                Print("Scaling up...", debug);
                currentScale += Time.fixedDeltaTime * scaleSpeed;
                float clampedScale = Mathf.Min(currentScale, maxScale, maxDistance);
                if (currentScale != clampedScale)
                {
                    Print("Start the laser!", debug);
                    currentScale = clampedScale;
                    casting = true;
                    if (hasDuration)
                        WindDownCoroutine();
                }
            }

            if (ShouldFollowCrossHair)
            {
                // Get the crosshair's target
                Vector3 targetedPosition = Crosshair.GetTargetedPosition(transform);
                // Restricted the target's direction to the X and Z axes.
                Vector3 restrictedDirection = (targetedPosition - transform.position).XZVector3().normalized;
                // Look at the restricted direction relative to the origin position.
                transform.LookAt(transform.position + restrictedDirection);
            }


            if (casting)
            {
                currentScale = Mathf.Min(maxScale, maxDistance);
            }

            UpdateScaling();
        }

        UpdateRayCastOffsets();
    }

    private void UpdateMaxDistance()
    {
        Ray ray = new(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, scalingLayerMask);

        int totalNum = 0;
        int numInLayer = 0;
        if (!shouldShootForever && hits.Length > 0)
        {
            maxDistance = 1000f;
            foreach (var hit in hits)
            {
                if (scalingLayerMask.LayerInMask(hit.collider.gameObject.layer))
                {
                    float old = maxDistance;
                    maxDistance = Mathf.Min(hit.distance, maxDistance);
                    numInLayer += 1;
                    Print($"Restricted: {old} -> {maxDistance} (by {hit.distance}, hit #{numInLayer})", debug);
                }
                else
                {
                    Print($"Hit didn't match {scalingLayerMask.value}, was {hit.collider.gameObject.layer}");
                }

                totalNum += 1;
            }
        }
        else
        {
            Print($"UnRestricted: {1000f}");
            maxDistance = 1000f;
        }

        Print($"Total hits: {totalNum}, # in layer: {numInLayer}");

        if (totalNum != 0) this.totalNum = totalNum;
        if (numInLayer != 0) this.numInLayer = totalNum;


        Vector3 rayMaxVector = ray.direction * (1000f - maxDistance);
        Vector3 collisionVector = ray.direction * maxDistance;
        Debug.DrawRay(ray.origin + collisionVector, rayMaxVector, Color.magenta, .1f);
        Debug.DrawRay(ray.origin,  collisionVector, Color.red, .1f);
    }
}

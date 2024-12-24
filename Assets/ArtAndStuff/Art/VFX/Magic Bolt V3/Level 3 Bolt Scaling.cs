using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Level3BoltScaling : BaseMonoBehaviour
{
    [Header("Configuration")]
    public new Collider collider;
    public VisualEffect bolt;
    public float scaleSpeed;
    public float castDuration;
    [SerializeField] private float maxScale = 50f;
    [SerializeField] private bool debug = false;
    [SerializeField] private bool debugEnable = false;

    [Header("State")]
    [ReadOnly][SerializeField] private bool playing = false;
    [ReadOnly][SerializeField] private bool casting = false;
    [SerializeField] private float currentScale;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private float collisionMargin = 0.01f;
    [SerializeField] private bool shouldFollowCrossHair;
    [SerializeField] private bool shouldShootForever;
    [SerializeField] private LayerMask raycastLayer;
    [SerializeField][ReadOnly] private int numInLayer;
    [SerializeField][ReadOnly] private int totalNum;
    private Coroutine windDownCoroutine;

    [Foldout("Events", true)]
    [SerializeField] protected UnityEvent onWoundDown = new();

    public bool ShouldFollowCrossHair
    {
        get => shouldFollowCrossHair;
        set => shouldFollowCrossHair = value;
    }

    // Start is called before the first frame update
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
        collider.enabled = true;
        playing = true;
        casting = false;
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
        bolt.SetBool("Effect End", true);
        windDownCoroutine = null;
        UpdateScaling();
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
        collider.transform.localScale = new Vector3(collider.transform.localScale.x, collider.transform.localScale.y, (currentScale * 100) + collisionMargin);
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
                    WindDownCoroutine();
                }
            }

            if (ShouldFollowCrossHair)
            {
                transform.LookAt(Crosshair.GetTargetedPosition(transform));
            }


            if (casting)
            {
                currentScale = Mathf.Min(maxScale, maxDistance);
            }

            UpdateScaling();
        }
    }

    private void UpdateMaxDistance()
    {
        Debug.Break();
        Ray ray = new(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, raycastLayer);

        int totalNum = 0;
        int numInLayer = 0;
        if (!shouldShootForever && hits.Length > 0)
        {
            maxDistance = 1000f;
            foreach (var hit in hits)
            {
                if (raycastLayer.LayerInMask(hit.collider.gameObject.layer))
                {
                    float old = maxDistance;
                    maxDistance = Mathf.Min(hit.distance, maxDistance);
                    numInLayer += 1;
                    Print($"Restricted: {old} -> {maxDistance} (by {hit.distance}, hit #{numInLayer})", debug);
                }
                else
                {
                    Print($"Hit didn't match {raycastLayer.value}, was {hit.collider.gameObject.layer}");
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

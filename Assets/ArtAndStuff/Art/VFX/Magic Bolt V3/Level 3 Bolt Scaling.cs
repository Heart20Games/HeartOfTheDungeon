using MyBox;
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
    [SerializeField] private bool shouldFollowCrossHair;
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
        collider.transform.localScale = new Vector3(collider.transform.localScale.x, collider.transform.localScale.y, (currentScale * 100));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playing)
        {
            Print("Playing...", debug);
            if (!casting)
            {
                Print("Scaling up...", debug);
                currentScale += Time.fixedDeltaTime * scaleSpeed;
                float clampedScale = Mathf.Min(currentScale, maxScale);
                if (currentScale != clampedScale)
                {
                    Print("Start the laser!", debug);
                    currentScale = clampedScale;
                    casting = true;
                    WindDownCoroutine();
                }
            }

            if(shouldFollowCrossHair)
            {
                transform.LookAt(Crosshair.GetTargetedPosition(transform));
            }
            
            UpdateScaling();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Level3BoltScaling : BaseMonoBehaviour
{
    [Header("Configuration")]
    public GameObject meshCollider;
    public VisualEffect bolt;
    public float scaleSpeed;
    public float castDuration;
    [SerializeField] private float maxScale = 50f;
    [SerializeField] private bool debug = true;

    [Header("State")]
    [ReadOnly][SerializeField] private bool playing = false;
    [ReadOnly][SerializeField] private bool casting = false;
    [SerializeField] private float currentScale;
    private Coroutine windDownCoroutine;

    // Start is called before the first frame update
    private void Start()
    {
        currentScale = 0f;
    }

    private void OnEnable()
    {
        if (!playing)
        {
            ChargeUp();
        }
    }

    private void OnDisable()
    {
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
        meshCollider.transform.localScale = new Vector3(meshCollider.transform.localScale.x, meshCollider.transform.localScale.y, (currentScale * 100));
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

            transform.LookAt(Crosshair.GetTargetedPosition());
            UpdateScaling();
        }
    }
}

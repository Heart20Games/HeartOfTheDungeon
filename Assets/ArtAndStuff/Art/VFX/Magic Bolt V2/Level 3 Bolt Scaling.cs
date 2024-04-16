using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Level3BoltScaling : MonoBehaviour
{
    public GameObject meshCollider;
    public VisualEffect bolt;
    public float scaleSpeed;
    public float castDuration;
    private bool playing = false;
    private bool casting = false;
    [SerializeField] private float currentScale;
    private Coroutine windDownCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        currentScale = 0f;
    }

    public void ChargeUp()
    {
        bolt.SetFloat("Duration", castDuration);
        bolt.SetBool("Effect End", false);
        bolt.Play();
        playing = true;
        currentScale = 0;
        InterruptWindDown();
    }

    public void InterruptWindDown()
    {
        if (windDownCoroutine != null)
        {
            StopCoroutine(windDownCoroutine);
            windDownCoroutine = null;
        }
    }

    public void WindDown()
    {
        InterruptWindDown();
        windDownCoroutine = StartCoroutine(DoWindDown(castDuration));
    }

    private IEnumerator DoWindDown(float duration)
    {
        yield return new WaitForSeconds(duration);
        bolt.Stop();
        currentScale = 0f;
        enabled = false;
        playing = false;
        casting = false;
        bolt.SetBool("Effect End", true);
        windDownCoroutine = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActiveAndEnabled)
        {
            if (!playing)
            {
                ChargeUp();
            }

            if (!casting)
            {
                currentScale += Time.fixedDeltaTime * scaleSpeed;
                float clampedScale = Mathf.Min(currentScale, 50f);
                if (currentScale != clampedScale)
                {
                    currentScale = clampedScale;
                    casting = true;
                    WindDown();
                }
                bolt.SetFloat("Scale", currentScale);
                meshCollider.transform.localScale = new Vector3(meshCollider.transform.localScale.x, meshCollider.transform.localScale.y, (currentScale * 100));
            }
        }
    }
}

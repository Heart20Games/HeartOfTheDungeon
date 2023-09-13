using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Charger : BaseMonoBehaviour
{
    public float rate = 1f;
    public float increment = 0.01f;
    public float length = 1f;
    public bool beginOnStart = false;

    [Header("Status")]
    [SerializeField] private bool interrupt = false;

    [Header("Events")]
    public UnityEvent onBegin;
    public UnityEvent<float> onCharge;
    public UnityEvent onCharged;
    public UnityEvent onInterrupt;

    private void Start()
    {
        if (beginOnStart) Begin();
    }

    public void Begin()
    {
        interrupt = false;
        onBegin.Invoke();
        StartCoroutine(ChargeTimer());
    }

    public void Interrupt()
    {
        interrupt = true;
    }

    public IEnumerator ChargeTimer()
    {
        float time = 0f;
        while (time < length)
        {
            onCharge.Invoke(time/length);
            float rate = Mathf.Min(length - time, increment);
            yield return new WaitForSeconds(rate);
            if (interrupt) break;
            time += rate;
        }
        if (!interrupt)
        {
            onCharge.Invoke(1);
            onCharged.Invoke();
        }
        else
        {
            interrupt = false;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Charger : BaseMonoBehaviour
{
    public float rate = 1f;
    public float increment = 0.01f;
    public float length = 1f;
    public bool beginOnStart = false;

    [Header("Status")]
    [ReadOnly][SerializeField] private float time = 0f;
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
        float rate = 0f;
        time = 0f;
        while (!interrupt)
        {
            onCharge.Invoke(time/length);
            if (time < length)
                rate = Mathf.Min(length - time, increment);
            else
                rate = increment;
            yield return new WaitForSeconds(rate);
            if (interrupt) break;
            time += rate;
            if (time >= length && time < (length + increment))
                onCharged.Invoke();
        }
        interrupt = false;
    }
}

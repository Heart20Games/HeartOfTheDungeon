using Attributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Charger : BaseMonoBehaviour
{
    public bool debug = false;

    [Header("Status")]
    [SerializeField] private bool interrupt = false;
    [ReadOnly][SerializeField] private int level;
    [ReadOnly][SerializeField] private int maxLevel;
    public DependentAttribute chargeLimit;
    public float[] chargeTimes = new float[0];

    [Header("Events")]
    public UnityEvent onBegin;
    public UnityEvent<float> onCharge;
    public UnityEvent onCharged;
    public UnityEvent onInterrupt;

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
        level = 0;
        while (!interrupt)
        {
            if (debug) print($"Charged to level {level}");
            onCharge.Invoke(level);
            float waitTime = level >= chargeTimes.Length ? 1 : chargeTimes[level];
            yield return new WaitForSeconds(waitTime);
            if (interrupt) break;
            maxLevel = (int)chargeLimit.FinalValue;
            if (level >= chargeLimit.FinalValue)
            {
                if (debug) print($"Fully Charged at {level} / {chargeLimit.FinalValue}");
                onCharged.Invoke(); break;
            }
            level++;
        }
        interrupt = false;
    }
}

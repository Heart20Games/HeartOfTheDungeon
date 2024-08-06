using Attributes;
using MyBox;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Charger : BaseMonoBehaviour
{
    public bool debug = false;

    public bool resetOnBegin;

    [Header("Status")]
    [SerializeField] private bool interrupt = false;
    [SerializeField] private int level;
    [SerializeField] private int maxLevel;
    public float[] chargeTimes = new float[0];

    [Foldout("Events", true)]
    public UnityEvent onBegin;
    public UnityEvent<float> onCharge;
    public UnityEvent<int> onChargeInt;
    public UnityEvent onCharged;
    [Foldout("Events")]
    public UnityEvent onInterrupt;

    public void SetMaxLevel(int level)
    {
        maxLevel = level;
        if (level > maxLevel)
        {
            level = maxLevel;
            onCharge.Invoke(level);
            onChargeInt.Invoke(level);
        }
    }

    public void Begin()
    {
        if (resetOnBegin)
            level = 0;
        interrupt = false;
        onBegin.Invoke();
        StartCoroutine(ChargeTimer());
    }

    public void Interrupt()
    {
        interrupt = true;
    }

    public void InitializeEvents()
    {
        onBegin = new();
        onCharge = new();
        onChargeInt = new();
        onCharged = new();
        onInterrupt = new();
    }

    public IEnumerator ChargeTimer()
    {
        level = 0;
        while (!interrupt)
        {
            if (debug) print($"Charged to level {level}");
            onCharge.Invoke(level);
            onChargeInt.Invoke(Mathf.FloorToInt(level));
            float waitTime = level >= chargeTimes.Length ? 1 : chargeTimes[level];
            yield return new WaitForSeconds(waitTime);
            if (interrupt) break;
            if (level >= maxLevel)
            {
                if (debug) print($"Fully Charged at {level} / {maxLevel}");
                onCharged.Invoke(); break;
            }
            level++;
        }
        interrupt = false;
    }
}

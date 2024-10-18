using Attributes;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Charger : BaseMonoBehaviour
{
    // Fields and Properties

    [SerializeField] private bool debug = false;

    public bool resetOnBegin;

    [Header("Settings")]
    [SerializeField] private bool interrupt = false;
    [SerializeField] private float level;
    [SerializeField] private float maxLevel;
    public bool discreteUpdates = false;
    public bool discharge = false;
    public bool distributeChargeTimes = false;
    private float chargeTimeScale = 1;
    public float[] chargeTimes = new float[0];

    [Foldout("Events", true)]
    public UnityEvent onBegin;
    public UnityEvent<float> onCharge;
    public UnityEvent<int> onChargeInt;
    public UnityEvent onCharged;
    [Foldout("Events")]
    public UnityEvent onInterrupt;

    // How to Give it Directions

    public void SetChargeTimeScale(float value)
    {
        chargeTimeScale = value;
    }

    public void SetMaxLevel(int level) => SetMaxLevel((float)level);
    public void SetMaxLevel(float level)
    {
        maxLevel = level;
        if (this.level > maxLevel)
        {
            this.level = maxLevel;
            onCharge.Invoke(this.level);
            onChargeInt.Invoke((int)this.level);
        }
    }

    public void Begin()
    {
        if (resetOnBegin)
            level = discharge ? maxLevel : 0;
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

    // The Stuff that Makes it Actually Happen

    private void SetRealLevel(ChargeLevels chargeLevels, float level)
    {
        if (chargeLevels.Count > 0)
        {
            int baseLevel = (int)level;
     
            float prev = baseLevel < 1 ? chargeLevels.startValue : chargeLevels[baseLevel-1].level;
            float cur = baseLevel >= chargeLevels.Count ? chargeLevels[^1].level : chargeLevels[baseLevel].level;
            float progress = level - baseLevel;
        
            this.level = Mathf.Lerp(prev, cur, progress);
        }
        else
        {
            this.level = discharge ? 0 : 1;
        }

        this.level = Mathf.Min(this.level, maxLevel);
        
        onCharge.Invoke(this.level);
        onChargeInt.Invoke(Mathf.FloorToInt(this.level));
    }

    [Header("Status")]
    [ReadOnly][SerializeField] private bool active;
    [SerializeField] private float chargeLevel;
    [ReadOnly][SerializeField] private float waitTime;
    [SerializeField] private ChargeLevels chargeLevels;

    public IEnumerator ChargeTimer()
    {
        active = true;

        // Note: Charging vs Discharging is decided by the order of the ChargeLevel list.
        chargeLevels = CalculateChargeLevels(maxLevel, chargeTimes);

        // Starting Charge Level
        chargeLevel = 0;
        
        while (!interrupt)
        {
            // Report the Current Level
            Print($"{(discharge ? "Discharged" : "Charged")} to level {chargeLevel}", debug);
            SetRealLevel(chargeLevels, chargeLevel);
            
            // Waiting (And Calculating Increments)
            waitTime = DetermineWaitTime(chargeLevels, chargeLevel);
            float levelIncrement;
            if (discreteUpdates)
            {
                yield return new WaitForSeconds(waitTime);
                levelIncrement = 1;
            }
            else
            {
                yield return null;
                levelIncrement = Mathf.Min(((int)level + 1) - level, Time.deltaTime / waitTime);
            }

            // Interruptions
            if (interrupt) break;
            
            // Updating the Charge Level
            if (chargeLevel >= chargeLevels.Count)
            {
                Print($"Fully Charged at {chargeLevel} / {chargeLevels.Count}", debug);
                Break(debug, this);
                onCharged.Invoke(); break;
            }
            chargeLevel = Mathf.Min(chargeLevel + levelIncrement, chargeLevels.Count);
        }
        interrupt = false;

        active = false;
    }

    private float DetermineWaitTime(ChargeLevels chargeLevels, float chargeLevel)
    {
        float waitTime = 0;
        if (chargeLevel < chargeLevels.Count)
        {
            waitTime = chargeLevels[(int)chargeLevel].chargeTime;
        }
        return waitTime;
    }

    private ChargeLevels CalculateChargeLevels(float maxLevel, float[] chargeTimes)
    {
        ChargeLevels chargeLevels = new(0);

        // Determine our method of Calculation.
        if (!distributeChargeTimes)
        {
            int level = 0;
            foreach (float chargeTime in chargeTimes)
            {
                level += 1;
                chargeLevels.Add(new(level, chargeTime));
            }
        }
        else
        {
            float totalChargeTime = 0;
            foreach (float chargeTime in chargeTimes)
            {
                totalChargeTime += chargeTime;
            }
        
            float time = 0;
            for (int i = 0; i < chargeTimes.Length; i++)
            {
                time += chargeTimes[i];
                var ratio = time / totalChargeTime;
                chargeLevels.Add(new(maxLevel * ratio, chargeTimes[i]));
            }
        }

        // Return ChargeLevels, possibly reversed for Discharging.
        return discharge switch
        {
            true => chargeLevels.Discharge(),
            false => chargeLevels
        };
    }

    // Charge Level Structs

    [Serializable]
    private struct ChargeLevel
    {
        public float level;
        public float chargeTime;

        public ChargeLevel(float level, float chargeTime)
        {
            this.level = level;
            this.chargeTime = chargeTime;
        }

        public ChargeLevel(float level, ChargeLevel old)
        {
            this.level = level;
            this.chargeTime = old.chargeTime;
        }
    }

    [Serializable]
    private struct ChargeLevels
    {
        public List<ChargeLevel> levels;
        public float startValue;

        public ChargeLevels(float startValue)
        {
            this.levels = new();
            this.startValue = 0;
        }

        public ChargeLevels(float startValue, ChargeLevels old)
        {
            this.levels = old.levels;
            this.startValue = startValue;
        }

        public readonly ChargeLevels Discharge()
        {
            if (levels.Count == 0) return this;
            
            // Shifts level values over one (account for the start value), then reverses the list.
            var level = levels[0];
            levels[0] = new(startValue, level);
            for (int i = 1; i < levels.Count; i++)
            {
                var next = levels[i];
                levels[i] = new(level.level, next);
                level = next;
            }
            levels.Reverse();
            return new(level.level, this);
        }

        public readonly ChargeLevel this[int index]
        {
            get => levels[index];
            set => levels[index] = value;
        }

        public readonly int Count => levels.Count;
        public readonly void Add(ChargeLevel level) => levels.Add(level);
    }
}

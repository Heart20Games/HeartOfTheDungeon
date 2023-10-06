using System;
using UnityEngine;
using UnityEngine.Events;

public class PropertyAdjuster : MonoBehaviour
{
    public int LoopMin { get => loopMin; set => loopMin = value; }
    public int LoopMax { get => loopMax; set => loopMax = value; }
    public int LoopLimit { get => loopLimit; set => loopLimit = value; }
    public int loopMin = 0;
    public int loopLimit = 3;
    public int loopMax = 3;

    [Serializable]
    public struct ConditionGate
    {
        public string property;
        public int conditionValue;
    }
    public ConditionGate[] conditionGates = new ConditionGate[0];
    
    public string formatString = "|";

    public UnityEvent<string, bool> onBool;
    public UnityEvent<string> onTrigger;

    // Bool Toggle
    public void ToggleOn(string property) { Toggle(property, true); }
    public void ToggleOff(string property) { Toggle(property, false); }
    public void Toggle(string property, bool value)
    {
        onBool.Invoke(property, value);
    }

    // Condition Gates
    public void ConditionOn(float value) { ConditionOn((int)value); }
    public void ConditionOn(int value) { Condition(value, true); }
    public void ConditionOff(float value) { ConditionOff((int)value); }
    public void ConditionOff(int value) { Condition(value, false); }
    public void Condition(int value, bool on)
    {
        foreach (ConditionGate gate in conditionGates)
        {
            if (gate.conditionValue == value)
            {
                onBool.Invoke(gate.property, on);
            }
            else
            {
                onBool.Invoke(gate.property, !on);
            }
        }
    }

    // Trigger
    public void Trigger(string property)
    {
        onTrigger.Invoke(property);
    }

    // Bool Loop
    public void ToggleOnLoop(string property)
    {
        ToggleLoop(property, true);
    }

    public void ToggleOffLoop(string property)
    {
        ToggleLoop(property, false);
    }



    public void ToggleLoop(string property, bool value)
    {
        for (int i = loopMin; i <= Mathf.Clamp(0, loopLimit, loopMax); i++)
        {
            Toggle(property.Replace(formatString, i.ToString()), value);
        }
        for (int i = loopLimit + 1; i <= loopMax; i++)
        {
            Toggle(property.Replace(formatString, i.ToString()), !value);
        }
    }
}

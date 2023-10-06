using MyBox;
using System;
using UnityEngine;
using UnityEngine.Events;

public class PropertyAdjuster : MonoBehaviour
{
    // Looping
    public int LoopMin { get => loopMin; set => loopMin = value; }
    public int LoopMax { get => loopMax; set => loopMax = value; }
    public int LoopLimit { get => loopLimit; set => loopLimit = value; }
    [Foldout("Looping")] public int loopMin = 0;
    [Foldout("Looping")] public int loopLimit = 3;
    [Foldout("Looping")] public int loopMax = 3;
    [Foldout("Looping")] public string formatString = "|";

    // Initials
    [Serializable]
    public struct Initial<T>
    {
        public string property;
        public T value;
    }
    [Foldout("Initials")] public Initial<bool>[] initialBools = new Initial<bool>[0];
    [Foldout("Initials")] public Initial<float>[] initialFloats = new Initial<float>[0];
    [Foldout("Initials")] public Initial<int>[] initialInts = new Initial<int>[0];

    // Conditions
    [Serializable] public struct ConditionGate
    {
        public string property;
        public int conditionValue;
    }
    public ConditionGate[] conditionGates = new ConditionGate[0];
    [Foldout("Connections")] public UnityEvent<string, bool> onBool;
    [Foldout("Connections")] public UnityEvent<string> onTrigger;
    [Foldout("Connections")] public UnityEvent<string, float> onFloat;
    [Foldout("Connections")] public UnityEvent<string, int> onInt;

    // Bool Toggle
    public void ToggleOn(string property) { Toggle(property, true); }
    public void ToggleOff(string property) { Toggle(property, false); }
    public void Toggle(string property, bool value)
    {
        onBool.Invoke(property, value);
    }

    // Initialize
    public void Initialize()
    {
        Initialize(initialBools, onBool);
        Initialize(initialFloats, onFloat);
        Initialize(initialInts, onInt);
    }

    public void Initialize<T>(Initial<T>[] initials, UnityEvent<string, T> onEvent)
    {
        foreach (var initial in initials)
        {
            if (initial.property.Contains(formatString))
                Loop(initial.property, initial.value, onEvent);
            else
                onEvent.Invoke(initial.property, initial.value);
        }
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

    public void FloatLoop(string property, float value)
    {

    }

    // Bool Loop
    public void ToggleOnLoop(string property) { ToggleLoop(property, true); }
    public void ToggleOffLoop(string property) { ToggleLoop(property, false); }
    public void ToggleLoop(string property, bool value)
    {
        Loop(property, value, onBool);
        Loop(property, !value, onBool, true);
    }

    public void Loop<T>(string property, T value, UnityEvent<string, T> onEvent, bool inverse=false)
    {
        for (int i = (inverse ? loopLimit+1 : loopMin); i <= (inverse ? loopMax : Mathf.Clamp(0, loopLimit, loopMax)); i++)
        {
            onEvent.Invoke(property.Replace(formatString, i.ToString()), value);
        }
    }
}

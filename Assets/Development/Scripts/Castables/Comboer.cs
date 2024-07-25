using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Comboer
 * 
 * Purpose:
 *  Upon receiving a change in step or getting enabled, the Comboer will enable only Combo Objects that match the current step.
 */

public class Comboer : BaseMonoBehaviour
{
    [Serializable]
    private struct ComboStep
    {
        public ComboStep(int step, GameObject comboObject)
        {
            this.name = $"{step}: {comboObject.name}";
            this.step = step;
            this.comboObject = comboObject;
        }
        public string name;
        public GameObject comboObject;
        public int step;
    }

    [SerializeField] private int step;
    [SerializeField] private List<ComboStep> comboSteps;
    [SerializeField] private bool debug;

    private void OnEnable()
    {
        SetStep(step);
    }

    public void AddStep(int step, GameObject comboObject)
    {
        comboSteps ??= new();
        comboSteps.Add(new ComboStep(step, comboObject));
    }

    public void SetStep(int step)
    {
        Print($"Combo step: {step}", debug, this);
        this.step = step;
        comboSteps ??= new();
        foreach (var comboStep in comboSteps)
        {
            comboStep.comboObject.SetActive(comboStep.step == step);
        }
    }

    [ButtonMethod]
    public void Refresh()
    {
        SetStep(step);
    }
}

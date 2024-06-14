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
    private readonly struct ComboStep
    {
        public ComboStep(int step, GameObject comboObject)
        {
            this.step = step;
            this.comboObject = comboObject;
        }
        public readonly GameObject comboObject;
        public readonly int step;
    }

    [SerializeField] private int step;
    [SerializeField] private List<ComboStep> comboSteps;

    private void OnEnable()
    {
        SetStep(step);
    }

    public void AddStep(int step, GameObject comboObject)
    {
        comboSteps.Add(new ComboStep(step, comboObject));
    }

    public void SetStep(int step)
    {
        foreach (var comboStep in comboSteps)
        {
            comboStep.comboObject.SetActive(comboStep.step == step);
        }
    }
}

using UnityEngine;
using Articy.Unity;
using FMODUnity;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

public class GameConditionTrigger : MonoBehaviour
{
    [SerializeField] private string condition;
    [SerializeField] private string otherTag;
    [SerializeField] public ArticyRef articyRef;
    [SerializeField] private List<Line> lines = new();

    [Serializable] public struct Line
    {
        public int lineNumber;
        public ArticyRef articyRef;
    }

    private int LineNumber
    {
        get
        {
            return lines[GameConditionsManager.GetGameCondition(condition)].lineNumber;
        }
    }

    private ArticyRef ArticyRef
    {
        get
        {
            return lines[GameConditionsManager.GetGameCondition(condition)].articyRef;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == otherTag)
        {
            Debug.Log(other.gameObject.tag);
            TriggerCalloutsCondition();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == otherTag)
        {
            Debug.Log(other.tag);
            TriggerCalloutsCondition();
        }
    }
    
    public void TriggerCalloutsCondition()
    {
        Debug.Log("I got called");
        if (GameConditionsManager.GetGameCondition(condition) < lines.Count)
        {
            RuntimeManager.StudioSystem.setParameterByName("WizardDuelCallouts", LineNumber);
            GameConditionsManager.CalloutCondition(condition, ArticyRef, LineNumber);
        }
    }
}
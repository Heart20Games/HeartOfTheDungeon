using UnityEngine;
using Articy.Unity;
using FMODUnity;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

public class GameConditionTrigger : MonoBehaviour
{
    [SerializeField] private string conditions;
    [SerializeField] private string otherTag;
    [SerializeField] public ArticyRef articyRef;
    [SerializeField] private List<Line> linesOsseus = new();
    [SerializeField] private List<Line> linesRotta = new();
    [SerializeField] private List<Line> linesBaelor = new();

    private string condition; 
    private int lineSpeaker = 0;
    private List<Line> lines;
    private int linesPlayed = 0;

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
            TriggerCalloutsCondition();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == otherTag)
        {
            //Debug.Log(other.tag);
            TriggerCalloutsCondition();
        }
    }
    
    public void TriggerCalloutsCondition()
    {
        if (DialogueManager.Instance.lookForStop == false)
        {
            CheckForLineCount();
        }
    }

    private void CheckForLineCount()
    {
        lineSpeaker = UnityEngine.Random.Range(0, 3);
        int totalLinesCount = linesOsseus.Count + linesRotta.Count + linesBaelor.Count;
        Debug.Log(totalLinesCount);
        if (linesPlayed >= totalLinesCount)
        {
            Debug.Log("Count Reached");
            return;
        }
        switch (lineSpeaker)
        {
            case 0:
                condition = conditions + "Osseus";
                lines = linesOsseus;
                if (GameConditionsManager.GetGameCondition(condition) >= lines.Count)
                {
                    Debug.Log("Osseus Count Reached");
                    TriggerCalloutsCondition();
                    return;
                }
                else
                {
                    linesPlayed++;
                }
                break;
            case 1:
                condition = conditions + "Rotta";
                lines = linesRotta;
                if (GameConditionsManager.GetGameCondition(condition) >= lines.Count)
                {
                    Debug.Log("Rotta Count Reached");
                    TriggerCalloutsCondition();
                    return;
                }
                else
                {
                    linesPlayed++;
                }
                break;
            case 2:
                condition = conditions + "Baelor";
                lines = linesBaelor;
                if (GameConditionsManager.GetGameCondition(condition) >= lines.Count)
                {
                    Debug.Log("Baelor Count Reached");
                    TriggerCalloutsCondition();
                    return;
                }
                else
                {
                    linesPlayed++;
                }
                break;
        }

        if (GameConditionsManager.GetGameCondition(condition) < lines.Count)
        {
            RuntimeManager.StudioSystem.setParameterByName("WizardDuelCallouts", LineNumber);
            GameConditionsManager.CalloutCondition(condition, ArticyRef, LineNumber);
        }
    }
}
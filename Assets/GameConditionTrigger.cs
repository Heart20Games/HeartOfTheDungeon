using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Articy.Unity;
using FMODUnity;

public class GameConditionTrigger : MonoBehaviour
{
    [SerializeField] private string otherTag;
    [SerializeField] public ArticyRef articyRef;
    [SerializeField] public string calloutID;
    [SerializeField] private float calloutLineNumber;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == otherTag)
        {
            RuntimeManager.StudioSystem.setParameterByName("WizardDuelCallouts", calloutLineNumber);
            GameConditionsManager.instance.CalloutCondition(GameConditionsManager.instance.playerHitByBeam, articyRef, calloutLineNumber);
        }
    }
     
    public void TriggerCalloutsCondition()
    {
        RuntimeManager.StudioSystem.setParameterByName("WizardDuelCallouts", calloutLineNumber);
        GameConditionsManager.instance.CalloutCondition(GameConditionsManager.instance.playerHitByBeam, articyRef, calloutLineNumber);
    }
}
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == otherTag)
        {
            //RuntimeManager.StudioSystem.setParameterByNameWithLabel("WizardDuelCallouts", calloutID);
            GameConditionsManager.instance.CalloutCondition(GameConditionsManager.instance.playerHitByBeam, CalloutReferences.instance.DBO1);
        }
    }
     
    public void TriggerCalloutsCondition()
    {
        GameConditionsManager.instance.CalloutCondition(GameConditionsManager.instance.playerHitByBeam, articyRef);
    }
}
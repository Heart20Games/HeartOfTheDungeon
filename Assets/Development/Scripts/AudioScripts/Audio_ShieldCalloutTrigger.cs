using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_ShieldCalloutTrigger : MonoBehaviour
{
    public void CallShieldImpactCallout()
    {
        Debug.Log("Called Shield Impact");
        GameConditionTrigger conditionTrigger = GetComponentInParent<GameConditionTrigger>();
        conditionTrigger.TriggerCalloutsCondition();
    }
}

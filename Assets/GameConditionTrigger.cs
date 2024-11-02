using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConditionTrigger : MonoBehaviour
{
    [SerializeField] private string otherTag;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == otherTag)
        {
            GameConditionsManager.instance.CalloutCondition(GameConditionsManager.instance.playerHitByBeam, CalloutReferences.instance.DBO1);
        }
    }
}

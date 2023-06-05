using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPipTester : BaseMonoBehaviour
{
    public HealthPips healthPips;
    public bool takeDamage;
    public int damageToTake;
    public bool healDamage;
    public int damageToHeal;

    // Update is called once per frame
    void Update()
    {
        if(takeDamage)
        {      
            healthPips.TakeDamage(damageToTake);            
        }

        if(healDamage)
        {
            healthPips.HealDamage(damageToHeal);
        }
        takeDamage = false;
        healDamage = false;
    }
}

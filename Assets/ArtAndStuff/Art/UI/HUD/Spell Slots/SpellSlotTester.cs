using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSlotTester : BaseMonoBehaviour
{
    public int spellCost;
    public int spellGain;
    public int addSlots;
    public bool callGain = false;
    public bool callCost = false;
    public bool addTotal = false;
    public SpellSlots spellSlots;
    
   

    // Update is called once per frame
    void Update()
    {
        if(callCost)
        {
            spellSlots.SpendSpellSlots(spellCost);        
        }

        if(callGain)
        {
            spellSlots.GainSpellSlots(spellGain);            
        }

        if(addTotal)
        {
            spellSlots.IncreaseSpellSlots(addSlots);
        }
        
        callCost = false;
        callGain = false;
        addTotal = false;
    }


}

using System.Collections;
using System.Collections.Generic;
using log4net.Core;
using UnityEngine;
using UnityEngine.VFX;

public class CastingVFXController : MonoBehaviour
{
    
    private VisualEffect castingVFX;
    [SerializeField] private bool castingEnd; //Immediately kills all VFX. Used when cancelling or completing casting
    [SerializeField] private float level1Charge; //0 to 1 scale that determines progress on charging of level 1 bolt
    [SerializeField] private float level2Charge; //0 to 1 scale that determines progress on charging of level 2 ice cone
    [SerializeField] private float level3Charge; //0 to 1 scale that determines progress on charging of level 3 ice storm
    [SerializeField] private float castingCharge; //0 to 1 scale that determines how far into the casting of the spell the VFX are
    [SerializeField] private float castingTime; //how long the spell will run before the casting VFX should be dismissed
    [SerializeField] private float castingWindUpTime = 0; //float representing additional charge on castingCharge before tracking 0 to 1. This is for a precasting wind-up. Only applied to level 3 casts. 
    [SerializeField] private float timeToDestruct = 1; //duration in seconds before the GameObject is destroyed after enabling castingEnd
    
    
    // Start is called before the first frame update
    void Start()
    {
        castingVFX = GetComponent<VisualEffect>();
        
    }

    private void OnEnable() //reset all parameters upon enabling the VFX
    {
        if (castingVFX != null)
        {
            castingVFX.SetFloat("Level 1 Charge", 0f);
            castingVFX.SetFloat("Level 2 Charge", 0f);
            castingVFX.SetFloat("Level 3 Charge", 0f);
            castingVFX.SetFloat("Casting", 0f);
            castingVFX.SetBool("CastingEnd", false);
            level1Charge = 0f;
            level2Charge = 0f;
            level3Charge = 0f;
            castingCharge = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(castingTime == 0f)
            castingTime = 0.1f;
        
        if(castingVFX != null)
        {
            if (castingEnd)
            {
                StartCoroutine(EndCasting());
            }
            else if (castingCharge >= castingTime + castingWindUpTime)
            {
                castingEnd = true;
            }
            else if (castingCharge > 0f && level3Charge >= 1f)
            {
                level3Charge = 1f;
                level2Charge = 1f;
                level1Charge = 1f;
                UpdateCharges();
            }
            else if (castingCharge > 0f && level3Charge < 1f && level2Charge >= 1f)
            {
                level3Charge = 0f;
                level2Charge = 1f;
                level1Charge = 1f;
                UpdateCharges();
            }
            else if (castingCharge > 0f && level2Charge < 1f && level1Charge >= 1f)
            {
                level3Charge = 0f;
                level2Charge = 0f;
                level1Charge = 1f;
                UpdateCharges();
            }            
            else if (level3Charge > 0f)
            {
                level1Charge = 1f;
                level2Charge = 1f;
                UpdateCharges();
            }
            else if (level2Charge > 0f)
            {
                level1Charge = 1f;
                UpdateCharges();
            }
            else
            {
                UpdateCharges();
            }
        }        
    }

    private void UpdateCharges()
    {
        castingVFX.SetFloat("Level 3 Charge", level3Charge);
        castingVFX.SetFloat("Level 2 Charge", level2Charge);
        castingVFX.SetFloat("Level 1 Charge", level1Charge);
        castingVFX.SetFloat("Casting", castingCharge);
    }

    IEnumerator EndCasting()
    {
        castingVFX.SetBool("CastingEnd", true);
        yield return new WaitForSeconds(timeToDestruct);
        Destroy(gameObject);
        Debug.Log("Should be destroyed now");
    }

}

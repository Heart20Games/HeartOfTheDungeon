using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class IceBoltCastingVFXController : MonoBehaviour
{
    
    private VisualEffect castingVFX;
    [SerializeField] private float level1Casting; //0 to 1 scale that determines progress on charging of level 1 bolt
    [SerializeField] private float level2Casting; //0 to 1 scale that determines progress on charging of level 2 ice cone
    [SerializeField] private float level3Casting; //0 to 1 scale that determines progress on charging of level 3 ice storm
    [SerializeField] private float casting; //0 to 1 scale that determines how far into the casting of the spell the VFX are
    [SerializeField] private bool castingEnd; //Immediately kills all VFX. Used when cancelling or completing casting
    [SerializeField] private float level1Charge;
    [SerializeField] private float level2Charge;
    [SerializeField] private float level3Charge;
    [SerializeField] private float castingCharge;
    [SerializeField] private bool castLevel1;
    [SerializeField] private bool castLevel2;
    [SerializeField] private bool castLevel3;
    [SerializeField] private float castingTime = 1f;
    [SerializeField] private bool currentlyCharging = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        castingVFX = GetComponent<VisualEffect>();
        
    }

    private void OnEnable() 
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
        if(castingVFX != null)
        {
            if (castingEnd)
            {
                EndCasting();
            }
            else if (level3Charge > 0f)
            {
                level1Charge = 1f;
                level2Charge = 1f;
                castingVFX.SetFloat("Level 3 Charge", level3Charge);
            }
            else if (level2Charge > 0f)
            {
                level1Charge = 1f;
                castingVFX.SetFloat("Level 2 Charge", level2Charge);
            }
            else
            {
            castingVFX.SetFloat("Level 1 Charge", level1Charge);
            }

            if (castLevel1 && level2Charge == 0f && !currentlyCharging)
            {
                StartCoroutine(Level1Charge());
            }

            if (castLevel2 && level3Charge == 0f && !currentlyCharging)
            {
                StartCoroutine(Level2Charge());
            }

            if (castLevel3 && !currentlyCharging)
            {
                StartCoroutine(Level3Charge());
            }
        }        
    }

    private void EndCasting()
    {
        castingVFX.SetBool("CastingEnd", true);
    }

    IEnumerator Level1Charge()
    {
        currentlyCharging = true;
        while(level1Charge < 1f)
        {
            level1Charge += .05f;
            yield return new WaitForSeconds(castingTime * Time.deltaTime);
        }
        currentlyCharging = false;
    }

    IEnumerator Level2Charge()
    {
        currentlyCharging = true;
        while(level2Charge < 1f)
        {
            level2Charge += .05f;
            yield return new WaitForSeconds(castingTime * Time.deltaTime);
        }
        currentlyCharging = false;
    }

    IEnumerator Level3Charge()
    {
        currentlyCharging = true;
        while(level3Charge < 1f)
        {
            level3Charge += .05f;
            yield return new WaitForSeconds(castingTime * Time.deltaTime);
        }
        currentlyCharging = false;
    }
}

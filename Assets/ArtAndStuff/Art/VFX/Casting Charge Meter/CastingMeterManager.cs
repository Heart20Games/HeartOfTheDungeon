using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MeterProgressTester : MonoBehaviour
{
    
    public VisualEffect castingMeterFill;
    public VisualEffect castingMeterBackground;
    [Range(0, 1)]public float meterProgress;
    public bool levelUp;
    [Range(0f, 1.2f)]private float level2LockDissolveAmount;
    [Range(0f, 1.2f)]private float level3LockDissolveAmount;
    public bool level2Available;
    private bool level2Dissolved;
    public bool level3Available;
    private bool level3Dissolved;
    [SerializeField] private float dissolveDuration = 1f;
    public bool cooldown = false;
    

    // Update is called once per frame
    void Update()
    {
        
        
        if(castingMeterBackground != null)
        {
            castingMeterBackground.SetFloat("Meter Progress", meterProgress);
        }
        
        if(castingMeterFill != null)
        {        
            castingMeterFill.SetFloat("Meter Progress", meterProgress);
            
            if (cooldown)
            {
                castingMeterFill.SetBool("Cooldown", true);
            }
            else
            {
                castingMeterFill.SetBool("Cooldown", false);
            }
            
            if (levelUp)
            {
                castingMeterFill.SendEvent("Level Up 2");
                levelUp = false;
            }
            if (level2Available && !level2Dissolved)
            {
                StartCoroutine(Level2Dissolve());
            }
            else if (!level2Available && level2Dissolved)
            {
                StartCoroutine(Level2Reform());
            }
            
            if (level3Available && !level3Dissolved)
            {
                StartCoroutine(Level3Dissolve());
            }
            else if (!level3Available && level3Dissolved)
            {
                StartCoroutine(Level3Reform());
            }
        }
    }

    private IEnumerator Level2Dissolve()
    {
        level2Dissolved = true;
        while (level2LockDissolveAmount < 1.2)
        {
            level2LockDissolveAmount += .05f;
            castingMeterFill.SetFloat("Level 2 Lock Dissolve", level2LockDissolveAmount);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        castingMeterFill.SetBool("Level 2 Available", true);
    }

    IEnumerator Level3Dissolve()
    {
        level3Dissolved = true;
        while (level3LockDissolveAmount < 1.2)
        {
            level3LockDissolveAmount += .05f;
            castingMeterFill.SetFloat("Level 3 Lock Dissolve", level3LockDissolveAmount);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        castingMeterFill.SetBool("Level 3 Available", true);
    }

    private IEnumerator Level2Reform()
    {
        level2Dissolved = false;
        castingMeterFill.SetBool("Level 2 Available", false);
        while (level2LockDissolveAmount > 0)
        {
            level2LockDissolveAmount -= .05f;
            castingMeterFill.SetFloat("Level 2 Lock Dissolve", level2LockDissolveAmount);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        
    }

    private IEnumerator Level3Reform()
    {
        level3Dissolved = false;
        castingMeterFill.SetBool("Level 3 Available", false);
        while (level3LockDissolveAmount > 0)
        {
            level3LockDissolveAmount -= .05f;
            castingMeterFill.SetFloat("Level 3 Lock Dissolve", level3LockDissolveAmount);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        
    }
}

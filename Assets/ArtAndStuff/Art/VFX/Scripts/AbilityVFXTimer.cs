using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class AbilityVFXTimer : MonoBehaviour
{
    
[SerializeField] private VisualEffect visualEffect;
[SerializeField] private string level1Parameter;
[SerializeField] private string level2Parameter;
[SerializeField] private string level3Parameter;
[SerializeField] private string castingParameter;
[SerializeField] private string castingActivate;
[SerializeField] private float level1Time;
[SerializeField] private float level2Time;
[SerializeField] private float level3Time;
[SerializeField] private float level1Value;
[SerializeField] private float level2Value;
[SerializeField] private float level3Value;
[SerializeField] private float castingValue;
[SerializeField] private float castingTime;
[SerializeField] private float castDelay;
[SerializeField] private bool level1ContinueCount;
[SerializeField] private bool level2ContinueCount;
[SerializeField] private bool level3ContinueCount;
[SerializeField] private float rate = .02f;
[SerializeField] private bool level1Available;
[SerializeField] private bool level2Available;
[SerializeField] private bool level3Available;
[SerializeField] private int level1Duration;
[SerializeField] private int level2Duration;
[SerializeField] private int level3Duration;
[SerializeField] private float castingDuration;
[SerializeField] private bool isCasting = false;
[SerializeField] private bool cast = false;


    private void FixedUpdate()
    {
       if(isCasting && !cast)
       {
            if(level1Time <= level1Duration && level1Available)
            {
                level1Value += rate;
                visualEffect.SetFloat(level1Parameter, level1Value);
                level1Time += .02f;
            }
            else if(level2Time <= level2Duration && level2Available)
            {
                level2Value += rate;
                visualEffect.SetFloat(level2Parameter, level2Value);
                level2Time += .02f;
            }
            else if(level3Time <= level3Duration && level3Available)
            {
                level3Value += rate;
                visualEffect.SetFloat(level3Parameter, level3Value);
                level3Time += .02f;
            }
       
            if(level1ContinueCount && level1Time > level1Duration)
            {
                level1Value += rate;
                visualEffect.SetFloat(level1Parameter, level1Value);
            }
            
            if(level2ContinueCount && level2Time > level2Duration)
            {
                level2Value += rate;
                visualEffect.SetFloat(level2Parameter, level2Value);
            }

            if(level3ContinueCount && level3Time > level3Duration)
            {
                level3Value += rate;
                visualEffect.SetFloat(level3Parameter, level3Value);
            }
       }
       else if (cast)
       {
            if(castingTime < castingDuration)
            {
                castingValue += rate;
                visualEffect.SetFloat(castingParameter, castingValue);
                castingTime += .02f;
            }
            else
            {
                StartCoroutine (Casting());
            }
       }

    }

    

    void CastBegin()
    {
        isCasting = true;
    }

    void CastEnd()
    {
        cast = true;
        isCasting = false;
    }

    IEnumerator Casting()
    {
        visualEffect.SetBool(castingActivate, true);
        yield return new WaitForSeconds (castDelay);
        level1Time = 0f;
        level2Time = 0f;
        level3Time = 0f;
        castingTime = 0f;
        level1Value = 0f;
        visualEffect.SetFloat(level1Parameter, level1Value);
        level2Value = 0f;
        visualEffect.SetFloat(level2Parameter, level2Value);
        level3Value = 0f;
        visualEffect.SetFloat(level3Parameter, level3Value);
        castingValue = 0f;
        visualEffect.SetFloat(castingParameter, castingValue);
        cast = false;
        isCasting = false;
        visualEffect.SetBool(castingActivate, false);
    }

}


    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MagicBolt_ChargingVFXScript : MonoBehaviour
{

    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private float level2ChargeTime;
    [SerializeField] private float level3ChargeTime;
    [SerializeField] private float castingChargeTime;
    [SerializeField] private float level2Charge;
    [SerializeField] private float level3Charge;
    [SerializeField] private float castingCharge;
    [SerializeField] public bool level2;
    [SerializeField] public bool level3;
    [SerializeField] public bool casting;
    [SerializeField] public bool castingEnd;
    [SerializeField] private bool isPlaying = false;


   

    // Update is called once per frame
    void Update()
    {
        if(castingEnd)
        {
            StartCoroutine (CastingEnd());
        }
        else if(casting)
        {
            Casting();
        }
        else if (level3)
        {
            Level3();
        }
        else if (level2)
        {
            Level2();
        }
    }

    IEnumerator CastingEnd()
    {
        visualEffect.SetBool("CastingEnd", true);
        yield return new WaitForSeconds (2);
        level2 = false;
        level3 = false;
        casting = false;
        castingEnd = false;
        level2Charge = 0f;
        level3Charge = 0f;
        castingCharge = 0f;
        visualEffect.SetFloat("Level 2 Charge", 0f);
        visualEffect.SetFloat("Level 3 Charge", 0f);
        visualEffect.SetFloat("Casting", 0f);
        visualEffect.SetBool("CastingEnd", false);
        visualEffect.Stop();
        isPlaying = false;
    }

    void Casting()
    {
        castingCharge += Time.deltaTime/castingChargeTime;
        visualEffect.SetFloat("Casting", castingCharge);
        Level3();
        Level2();

    }

    void Level3()
    {
        level3Charge += Time.deltaTime/level3ChargeTime;
        visualEffect.SetFloat("Level 3 Charge", level3Charge);
        Level2();
    }

    void Level2()
    {
        if(!isPlaying)
        {
            isPlaying = true;
            visualEffect.Play();
        }
        level2Charge += Time.deltaTime/level2ChargeTime;
        visualEffect.SetFloat("Level 2 Charge", level2Charge);
    }


}

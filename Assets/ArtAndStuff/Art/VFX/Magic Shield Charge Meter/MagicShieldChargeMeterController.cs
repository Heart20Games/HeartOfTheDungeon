using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MagicShieldChargeMeterController : MonoBehaviour
{

    [SerializeField] private VisualEffect meterVFX;
    [Range(0f, 1f)]public float meterPosition;
    public bool shieldBreak = false;
    private bool shieldBroken = false;
    public bool shieldRestore = false;
    //0 to 1f VFX parameter that determines the alpha level of the cracks
    private float crackAlpha;
    //0.1f to 1f VFX parameter that determines the alpha level of the top layer cracks
    private float topCrackFade;
    //-1f to 0f VFX parameter that increases the intensity of the cracks as it goes lower
    private float breakModifier;
    [SerializeField] private float crackFadeSpeed = 2;


    // Update is called once per frame
    void Update()
    {
        if (meterVFX != null)
        {
            meterVFX.SetFloat("Mask Position", meterPosition);
            if (shieldBreak && !shieldBroken)
            {
                StartCoroutine(ShieldBreak());
            }
            if (shieldRestore)
            {
                StartCoroutine(ShieldRestore());
            }
        }
    }

    private IEnumerator ShieldBreak()
    {
        shieldBroken = true;
        topCrackFade = 1f;
        crackAlpha = 0f;
        breakModifier = -1f;
        meterVFX.SetBool("ShieldBreak", true);
        meterVFX.SendEvent("ShieldBreak");
        while (crackAlpha < 1 && !shieldRestore)
        {
            crackAlpha += .05f;
            topCrackFade -= .05f;
            breakModifier += .05f;
            crackAlpha = Mathf.Clamp(crackAlpha, 0f, 1f);
            topCrackFade = Mathf.Clamp(topCrackFade, 0.01f, 1f);
            breakModifier = Mathf.Clamp(breakModifier,-1f, 0f);
            Debug.Log(topCrackFade);
            meterVFX.SetFloat("CrackAlphaClipping", crackAlpha);
            meterVFX.SetFloat("TopLayerCrackErosion", topCrackFade);
            meterVFX.SetFloat("BreakModifier", breakModifier);
            yield return new WaitForSeconds(crackFadeSpeed/75f);
        }
        breakModifier = 0f;
        
    }

    private IEnumerator ShieldRestore()
    {        
        shieldBroken = false;
        shieldBreak = false;
        shieldRestore = false;
        meterVFX.SendEvent("ShieldRestore");
        while (crackAlpha > 0 && !shieldBroken)
        {
            crackAlpha -= .05f;
            topCrackFade += .05f;
            crackAlpha = Mathf.Clamp(crackAlpha, 0f, 1f);
            topCrackFade = Mathf.Clamp(topCrackFade, 0.01f, 1f);
            meterVFX.SetFloat("CrackAlphaClipping", crackAlpha);
            meterVFX.SetFloat("TopLayerCrackErosion", topCrackFade);
            yield return new WaitForSeconds(crackFadeSpeed/50f);
        }
        meterVFX.SetBool("ShieldBreak", false);
    }
}

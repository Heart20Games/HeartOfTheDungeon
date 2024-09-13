using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MagicShieldChargeMeterController : MonoBehaviour
{

    [SerializeField] private VisualEffect meterVFX;
    [Range(-0.1f, 1.1f)]public float meterPosition;
    public bool shieldBreak = false;
    private bool shieldBroken = false;
    public bool shieldRestore = false;


    // Update is called once per frame
    void Update()
    {
        if (meterVFX != null)
        {
            meterVFX.SetFloat("Mask Position", meterPosition);
            if (shieldBreak)
            {
                ShieldBreak();
            }
            if (shieldRestore)
            {
                ShieldRestore();
            }
        }
    }

    void ShieldBreak()
    {
        meterVFX.SetBool("ShieldBreak", true);
        meterVFX.SendEvent("ShieldBreak");
        shieldBroken = true;
    }

    void ShieldRestore()
    {
        meterVFX.SetBool("ShieldBreak", false);
        shieldBroken = false;
        shieldBreak = false;
        shieldRestore = false;
    }
}

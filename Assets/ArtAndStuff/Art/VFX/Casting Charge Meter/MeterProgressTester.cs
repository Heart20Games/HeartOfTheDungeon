using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MeterProgressTester : MonoBehaviour
{
    
    public VisualEffect castingMeterFill;
    public VisualEffect castingMeterBackground;
    [Range(0, 1)]public float meterProgress;
    

    // Update is called once per frame
    void Update()
    {
        castingMeterFill.SetFloat("Meter Progress", meterProgress);
        castingMeterBackground.SetFloat("Meter Progress", meterProgress);
    }
}

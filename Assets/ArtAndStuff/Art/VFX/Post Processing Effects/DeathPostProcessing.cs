using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

public class DeathPostProcessing : MonoBehaviour
{
    private VolumeProfile volumeProfile;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    public float startingSaturation; // 100 is fully saturated, -100 is full black and white.
    [SerializeField] private float targetSaturation;
    [SerializeField] private float targetVignetteIntensity; // 0 is no vignette, 1 max vignette.
    private float currentSaturation;
    private float currentVignetteIntensity;
    [SerializeField] private float saturationRate; // How quickly it transitions from starting to target (amount per refresh rate).
    [SerializeField] private float vignetteIntensityRate; // How quickly it transitions from 0 to target.
    [SerializeField] private float refreshRate; // Time in seconds till refreshing.
    [SerializeField] private bool testDeath;
    [SerializeField] private bool testDeathDeactivate;
    
    // Start is called before the first frame update
    void Start()
    {
        volumeProfile = GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if(!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
       
        if(!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        if(!volumeProfile.TryGet(out colorAdjustments)) throw new System.NullReferenceException(nameof(colorAdjustments));
 
        vignette.intensity.Override(0f); 

        colorAdjustments.saturation.Override(startingSaturation);  
    }

    // Update is called once per frame
    void Update()
    {
        if(testDeath)
        {
            Death();
            testDeath = false;
        }

        if(testDeathDeactivate)
        {
            Alive();
            testDeathDeactivate = false;
        }
    }

    public void Death()
    {
        StartCoroutine (DeathActivate());
    }

    public void Alive()
    {
        StartCoroutine (DeathDeactivate());
    }

    IEnumerator DeathDeactivate()
    {
        currentSaturation =  colorAdjustments.saturation.value;
        currentVignetteIntensity =  vignette.intensity.value;
        
        while(currentSaturation < startingSaturation || currentVignetteIntensity > 0f)
        {
            if(currentSaturation < startingSaturation)
            {
                colorAdjustments.saturation.Override(currentSaturation - saturationRate);
                currentSaturation =  colorAdjustments.saturation.value;
            }
            if(currentVignetteIntensity > 0f)
            {
                vignette.intensity.Override(currentVignetteIntensity - vignetteIntensityRate);
                currentVignetteIntensity =  vignette.intensity.value;
            }

            yield return new WaitForSeconds (refreshRate);
        }
        
    }

    IEnumerator DeathActivate()
    {
        currentSaturation =  colorAdjustments.saturation.value;
        currentVignetteIntensity =  vignette.intensity.value;
        
        while(currentSaturation > targetSaturation || currentVignetteIntensity < targetVignetteIntensity)
        {
            if(currentSaturation > targetSaturation)
            {
                colorAdjustments.saturation.Override(currentSaturation + saturationRate);
                currentSaturation =  colorAdjustments.saturation.value;
            }
            if(currentVignetteIntensity < targetVignetteIntensity)
            {
                vignette.intensity.Override(currentVignetteIntensity + vignetteIntensityRate);
                currentVignetteIntensity =  vignette.intensity.value;
            }

            yield return new WaitForSeconds (refreshRate);
        }
        
    }

}




    
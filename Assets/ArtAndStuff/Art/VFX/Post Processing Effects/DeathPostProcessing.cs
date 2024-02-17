using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DeathPostProcessing : MonoBehaviour
{
    // Parts
    private VolumeProfile volumeProfile;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    [Header("Saturation")]
    [Tooltip("The deactivated saturation level. -100 is black and white, 100 is fully saturated.")]
    [Range(-100, 100)] public float startingSaturation = 24.3f; // 100 is fully saturated, -100 is full black and white.
    [Tooltip("The activated saturation level. -100 is black and white, 100 is fully saturated.")]
    [Range(-100, 100)][SerializeField] private float targetSaturation = -100;
    [Tooltip("The number of seconds till the saturation is at it's target.")]
    [Range(0.01f, 20)][SerializeField] private float saturationRate = 0.4143f; // How quickly it transitions from starting to target (amount per refresh rate).

    [Header("Vignette")]
    [Tooltip("The highest intensity the vignette should reach. 0 is off, 1 is maxed out.")]
    [Range(0, 1)][SerializeField] private float targetVignetteIntensity = 0; // 0 is no vignette, 1 max vignette.
    [Tooltip("The number of seconds till the vignette reaches it's target from 0.")]
    [Range(0.001f, 1f)][SerializeField] private float vignetteIntensityRate = 1f; // How quickly it transitions from 0 to target.

    [Header("Color Filter")]
    [SerializeField] private Color initialColor = Color.white;
    [SerializeField] private Color targetColor = Color.black;
    [Tooltip("The number of seconds till the color filter reaches target from intitial.")]
    [SerializeField] private float colorRate = 0.5f;
    [SerializeField] private float colorDelay = 1f;

    [Space]
    [Tooltip("The number of refreshes per second. (think of it like the \"framerate\")")]
    [Range(0.01f, 200f)][SerializeField] private float refreshRate = 50f; // Refreshes per second.
    
    // Current values
    private ClampedFloatParameter Saturation { get => colorAdjustments.saturation; }
    private ColorParameter Filter { get => colorAdjustments.colorFilter; }
    private ClampedFloatParameter Intensity { get => vignette.intensity; }
    
    // Start is called before the first frame update
    void Start()
    {
        // Get Components
        volumeProfile = GetComponent<Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(VolumeProfile));
        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
        if (!volumeProfile.TryGet(out colorAdjustments)) throw new System.NullReferenceException(nameof(colorAdjustments));
        
        // Initialize Overrides
        Intensity.Override(0f); 
        Saturation.Override(startingSaturation);
        Filter.Override(initialColor);
    }

    [ButtonMethod]
    public void Death()
    {
        StartCoroutine(Transition(true));
    }

    [ButtonMethod]
    public void Alive()
    {
        StartCoroutine(Transition(false));
    }

    private float HighestRate(bool activate=true)
    {
        return Mathf.Max(saturationRate, vignetteIntensityRate, activate ? colorDelay + colorRate : colorRate);
    }

    private void TransitionStep(float time, bool activate=true)
    {
        float startSat = activate ? startingSaturation : targetSaturation, endSat = activate ? targetSaturation : startingSaturation;
        float startInt = activate ? 0 : targetVignetteIntensity, endInt = activate ? targetVignetteIntensity : 0;
        Color startColor = activate ? initialColor : targetColor, endColor = activate ? targetColor : initialColor;

        Saturation.Override(Mathf.Lerp(startSat, endSat, time / saturationRate));
        Intensity.Override(Mathf.Lerp(startInt, endInt, time / vignetteIntensityRate));
        if (activate && time >= colorDelay)
            Filter.Override(Color.Lerp(startColor, endColor, (time - colorDelay) / colorRate));
        else if (!activate)
            Filter.Override(Color.Lerp(startColor, endColor, time / colorRate));
    }

    IEnumerator Transition(bool activate)
    {
        float startTime = Time.time;
        float time = Time.time - startTime;
        float endTime = HighestRate(activate);

        while (time < endTime)
        {
            TransitionStep(time, activate);
            yield return new WaitForSeconds(1 / refreshRate);
            time = Time.time - startTime;
        }

        TransitionStep(endTime, activate);
    }
}




    
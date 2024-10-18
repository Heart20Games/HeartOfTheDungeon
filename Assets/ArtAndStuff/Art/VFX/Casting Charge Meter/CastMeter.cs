using HotD.Castables;
using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class CastMeter : BaseMonoBehaviour
{
    // Fields
    [SerializeField] private bool debug;

    [Header("Meter Parameters")]
    [SerializeField] private VisualEffect meterFill;
    [SerializeField] private VisualEffect meterBackground;
    [SerializeField][Range(0, 1)] private int baseProgress = 0;
    [SerializeField][Range(-1, 1)] private int progressOffset = -1;
    [SerializeField][Range(0, 3)] private int progressCap;
    [SerializeField][Range(0, 3)] private int maxLevel;
    [SerializeField][Range(0, 3)] private int minLevel;
    [SerializeField] private bool shootSparks;

    [Header("Meter Progress")]
    [SerializeField][Range(0, 1)][ReadOnly] private float rawProgress;
    [SerializeField][Range(0, 1)][ReadOnly] private float offsetProgress;
    [SerializeField][Range(0, 1)] private float meterProgress;

    [Header("Cast Properties")]
    [SerializeField][ReadOnly] private bool hasCastProperties;
    [SerializeField][ReadOnly] private CastProperties rawCastProperties;
    private ICastProperties castProperties;
    
    [Foldout("Dissolve Parameters", true)]
    [SerializeField][Range(0f, 1.2f)] private float level2LockDissolveAmount;
    [SerializeField][Range(0f, 1.2f)] private float level3LockDissolveAmount;
    [SerializeField][ReadOnly] private bool level2Dissolved;
    [SerializeField][ReadOnly] private bool level3Dissolved;
    [SerializeField] private float dissolveDuration = 1f;
    [Foldout("Dissolve Parameters")]
    private bool cooldown = false;


    // Setters / Getters
    private void SetProgress(float rawProgress) { Progress = rawProgress; }
    private float Progress
    {
        get => UpdateProgress();
        set
        {
            rawProgress = value;
            UpdateProgress();
        }
    }
    private float UpdateProgress()
    {
        float oldProgress = meterProgress;
        offsetProgress = progressOffset + rawProgress;
        meterProgress = Mathf.Clamp(offsetProgress, baseProgress, ProgressCap) / maxLevel;

        float sparksThreshold = (float)((int)Mathf.Lerp(0, maxLevel, oldProgress) + 1) / maxLevel;
        if (meterProgress >= sparksThreshold)
        {
            Print($"CastMeter Leveled Up! ({oldProgress} -> {meterProgress}; {sparksThreshold})", debug, this);
            shootSparks = true;
        }
        return meterProgress;
    }

    private void SetProgressCap(int cap) { ProgressCap = cap; }
    private int ProgressCap
    {
        get => Mathf.Clamp(progressCap, minLevel, maxLevel);
        set => progressCap = Mathf.Max(value, 0);
    }

    public ICastProperties Castable { get => castProperties; set => SetCastable(value); }
    public void SetCastable(ICastProperties castProperties)
    {
        if (this.castProperties != null)
        {
            this.castProperties.FieldEvents.onSetPowerLevel.RemoveListener(SetProgress);
            this.castProperties.FieldEvents.onSetMaxPowerLevel.RemoveListener(SetProgressCap);
        }

        this.castProperties = castProperties;
        this.rawCastProperties = castProperties is CastProperties ? castProperties as CastProperties : null;
        hasCastProperties = this.castProperties != null;
        
        castProperties.FieldEvents.onSetPowerLevel.AddListener(SetProgress);
        castProperties.FieldEvents.onSetMaxPowerLevel.AddListener(SetProgressCap);

        Progress = castProperties.PowerLevel;
        ProgressCap = castProperties.MaxPowerLevel;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (meterFill != null)
        {
            SetFloat(meterFill, "Meter Progress", Progress);

            if (shootSparks)
            {
                meterFill.SendEvent("Level Up 2");
                shootSparks = false;
            }
        }

        if (meterBackground != null)
        {
            SetFloat(meterBackground, "Meter Progress", Progress);
        }

        // Dissolving and Reforming
        if (progressCap >= 2 && !level2Dissolved)
            StartCoroutine(Level2Dissolve());
        else if (progressCap < 2 && level2Dissolved)
            StartCoroutine(Level2Reform());

        if (progressCap >= 3 && !level3Dissolved)
            StartCoroutine(Level3Dissolve());
        else if (progressCap < 3 && level3Dissolved)
            StartCoroutine(Level3Reform());
    }

    private IEnumerator Level2Dissolve()
    {
        level2Dissolved = true;
        while (level2LockDissolveAmount < 1.2)
        {
            level2LockDissolveAmount += .05f;
            SetFloat(meterFill, "Level 2 Lock Dissolve", level2LockDissolveAmount);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        SetBool(meterFill, "Level 2 Available", true);
    }

    IEnumerator Level3Dissolve()
    {
        level3Dissolved = true;
        while (level3LockDissolveAmount < 1.2)
        {
            level3LockDissolveAmount += .05f;
            SetFloat(meterFill, "Level 3 Lock Dissolve", level3LockDissolveAmount);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        meterFill.SetBool("Level 3 Available", true);
    }

    private IEnumerator Level2Reform()
    {
        level2Dissolved = false;
        SetBool(meterFill, "Level 2 Available", false);
        while (level2LockDissolveAmount > 0)
        {
            level2LockDissolveAmount -= .05f;
            SetFloat(meterFill, "Level 2 Lock Dissolve", level2LockDissolveAmount);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        
    }

    private IEnumerator Level3Reform()
    {
        level3Dissolved = false;
        SetBool(meterFill, "Level 3 Available", false);
        while (level3LockDissolveAmount > 0)
        {
            level3LockDissolveAmount -= .05f;
            SetFloat(meterFill, "Level 3 Lock Dissolve", level3LockDissolveAmount);
            yield return new WaitForSeconds(dissolveDuration/50f);
        }
        
    }

    // Property Setters
    private void SetBool(VisualEffect visualEffect, string property, bool value)
    {
        if (visualEffect.HasBool(property))
            visualEffect.SetBool(property, value);
        else
            Debug.LogWarning("Cannot find Bool property on Visual Effect");
    }

    private void SetFloat(VisualEffect visualEffect, string property, float value)
    {
        if (visualEffect.HasFloat(property))
            visualEffect.SetFloat(property, value);
        else
            Debug.LogWarning("Cannot find Float property on Visual Effect");
    }
}

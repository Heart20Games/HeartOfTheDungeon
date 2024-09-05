using HotD.Castables;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MeterProgressManager : BaseMonoBehaviour
{
    
    public VisualEffect meterFill;
    public VisualEffect meterBackground;
    [Range(0, 1)] public float meterProgress;
    [Range(1, 3)] public int currentLevel;
    [Range(1, 3)] public int maxLevel;
    public bool levelUp;
    [Range(0f, 1.2f)]private float level2LockDissolveAmount;
    [Range(0f, 1.2f)]private float level3LockDissolveAmount;
    private bool level2Dissolved;
    private bool level3Dissolved;
    [SerializeField] private float dissolveDuration = 1f;
    private ICastProperties castPorperties;
    
    public ICastProperties Castable { get => castPorperties; set => SetCastable(value); }

    public void SetCastable(ICastProperties castProperties)
    {
        castPorperties = castProperties;
        castProperties.FieldEvents.onSetPowerLevel.AddListener(SetCastProgress);
        castProperties.FieldEvents.onSetMaxPowerLevel.AddListener(SetMaxLevel);
        castProperties.FieldEvents.onSetMaxPowerLevel.AddListener(SetCurrentLevel);
    }
    private void SetCastProgress(int castProgress)
    {
        meterProgress = castProgress / maxLevel;
    }
    private void SetCurrentLevel(int currentLevel)
    {
        this.currentLevel = currentLevel;
    }
    private void SetMaxLevel(int maxLevel)
    {
        this.maxLevel = maxLevel;
    }
    

    // Update is called once per frame
    void Update()
    {
        maxLevel = Mathf.Max(maxLevel, 1);
        currentLevel = Mathf.Min(Mathf.Max(currentLevel, 1), maxLevel);
        meterProgress = Mathf.Min(meterProgress, currentLevel/(float)maxLevel);

        if (meterFill != null)
        {
            SetFloat(meterFill, "Meter Progress", meterProgress);

            if (levelUp)
            {
                meterFill.SendEvent("Level Up 2");
                levelUp = false;
            }
        }

        if (meterBackground != null)
        {
            SetFloat(meterBackground, "Meter Progress", meterProgress);
        }

        // Dissolving and Reforming
        if (currentLevel >= 2 && !level2Dissolved)
            StartCoroutine(Level2Dissolve());
        else if (currentLevel < 2 && level2Dissolved)
            StartCoroutine(Level2Reform());

        if (currentLevel >= 3 && !level3Dissolved)
            StartCoroutine(Level3Dissolve());
        else if (currentLevel < 3 && level3Dissolved)
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

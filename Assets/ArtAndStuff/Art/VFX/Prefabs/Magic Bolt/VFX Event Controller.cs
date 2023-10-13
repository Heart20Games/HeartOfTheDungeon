using System.Collections;
using System.Collections.Generic;
using HotD.Castables;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;


public class VFXEventController : MonoBehaviour
{

    [SerializeField] private Animator vfxAnimator;
    [SerializeField] private Animator animator;
    [SerializeField] private Casted vfx;
    private VisualEffect vfxProperty;
    [SerializeField] private int minCastLevel = 2;
    [SerializeField] private int maxCastLevel = 3;


   
   private void Awake()
    {
        vfx = GetComponentInChildren<Casted>(true);
        vfxAnimator = vfx.GetComponent<Animator>();
        animator = this.GetComponent<Animator>();
        vfxProperty = GetComponentInChildren<VisualEffect>(true);

        
        //vfx.gameObject.SetActive(false);
        vfxAnimator.enabled = false;
        vfxProperty.enabled = false;
        vfx.onSetPowerLimit.AddListener(SetCastLevel);
        vfx.onTrigger.AddListener(TriggerAnimations);
        vfx.onCast.AddListener((Vector3 v)=>{Cast();});
    }

    public void SetCastLevel(int level)
    {   
        for(int i = minCastLevel; i <= maxCastLevel; i++)
        {
            animator.SetBool($"Level{i}", i <= level);       
            vfxAnimator.SetBool($"Level {i} Available", i <= level);
        }        
    }

    public void TriggerAnimations()
    {
        animator.SetTrigger("MagicBolt");
        vfx.gameObject.SetActive(true);
        vfxAnimator.enabled = true;
        vfxProperty.enabled = true;
    }

    public void MagicBoltLevel1()
    {
        animator.SetBool("Level2", false);
        animator.SetBool("Level3", false);
        vfxAnimator.SetBool("Level 2 Available", false);
        vfxAnimator.SetBool("Level 3 Available", false);
        animator.SetTrigger("MagicBolt");
        vfx.gameObject.SetActive(true);
        vfxAnimator.enabled = true;
        vfxProperty.enabled = true;
    }

    public void MagicBoltLevel2()
    {
        animator.SetBool("Level2", true);
        animator.SetBool("Level3", false);
        vfxAnimator.SetBool("Level 2 Available", true);
        vfxAnimator.SetBool("Level 3 Available", false);
        animator.SetTrigger("MagicBolt");
        vfx.gameObject.SetActive(true);
        vfxAnimator.enabled = true;
        vfxProperty.enabled = true;
    }

    public void MagicBoltLevel3()
    {
        animator.SetBool("Level2", true);
        animator.SetBool("Level3", true);
        vfxAnimator.SetBool("Level 2 Available", true);
        vfxAnimator.SetBool("Level 3 Available", true);
        animator.SetTrigger("MagicBolt");
        vfx.gameObject.SetActive(true);
        vfxAnimator.enabled = true;
        vfxProperty.enabled = true;
    }

    public void BeginCharge()
    {
        vfx.gameObject.SetActive(true);
        vfxAnimator.enabled = true;
        vfxProperty.enabled = true;
    }

    public void vfxCast()
    {
        vfxAnimator.SetTrigger("Cast");
    }

    public void Cast()
    {
        animator.SetTrigger("CastRelease");
        vfxAnimator.SetTrigger("Cast");
    }

    public void EndCast()
    {
        vfx.gameObject.SetActive(false);
        vfxAnimator.enabled = false;
        vfxProperty.enabled = false;
    }

}
using System.Collections;
using System.Collections.Generic;
using Body;
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
    [SerializeField] private Movement movement;
    [SerializeField] private Rigidbody character;
    

   
   private void Awake()
    {
        vfx = GetComponentInChildren<Casted>(true);
        vfxAnimator = vfx.GetComponent<Animator>();
        animator = this.GetComponent<Animator>();
        vfxProperty = GetComponentInChildren<VisualEffect>(true);
        movement = GetComponentInParent<Movement>();
        character = GetComponentInParent<Rigidbody>();

        
        vfx.gameObject.SetActive(false);
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
        vfx.gameObject.SetActive(true);
        vfxProperty.enabled = true;
        vfxAnimator.enabled = true;
        animator.SetTrigger("MagicBolt");   
        // vfxAnimator.Rebind();
        // vfxAnimator.Update(0f);
        // vfxAnimator.StartPlayback();
    }

    public void BeginCharge()
    {
        
        vfx.gameObject.SetActive(true);
        vfxProperty.enabled = true;
        vfxAnimator.enabled = true;
        
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
        StartMovement();
    }

    public void StopMovement()
    {
        movement.enabled = false;
        character.velocity = new Vector3(0,0,0);
    }

    public void StartMovement()
    {
        movement.enabled = true;
    }
}
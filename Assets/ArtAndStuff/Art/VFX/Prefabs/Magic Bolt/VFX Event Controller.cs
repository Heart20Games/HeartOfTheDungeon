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
    [SerializeField] private GameObject firePoint1;
    [SerializeField] private GameObject firePoint2;
    [SerializeField] private GameObject level3Beam;
    [SerializeField] private Level3BoltScaling level3BeamController;
    

   
   private void Start()
    {        
        animator = GetComponent<Animator>();
        vfx = firePoint1.GetComponentInChildren<Casted>(true);
        vfxAnimator = vfx.GetComponent<Animator>();
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

    public void VFXFirePoint2()
    {
        vfx.transform.parent = firePoint2.transform;
        vfx.transform.position = firePoint2.transform.position;
        //vfx.transform.Rotate(firePoint1.transform.rotation.x, firePoint1.transform.rotation.y, firePoint1.transform.rotation.z + 90);
    }

    public void VFXFirePoint1()
    {
        vfx.transform.parent = firePoint1.transform;
        
        vfx.transform.position = new Vector3(0,0,0);        
    }

    public void FireLevel3Beam()
    {
        level3Beam.SetActive(true);
        level3BeamController.cast = true;
    }

    public void EndBeam()
    {
        level3Beam.SetActive(false);
    } 
}
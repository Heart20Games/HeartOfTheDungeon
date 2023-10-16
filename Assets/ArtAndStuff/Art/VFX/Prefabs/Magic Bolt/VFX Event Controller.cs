using System.Collections;
using System.Collections.Generic;
using Body;
using HotD.Castables;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;


public class VFXEventController : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private int minCastLevel = 2;
    [SerializeField] private int maxCastLevel = 3;
    [ReadOnly][SerializeField] private int latestCastLevel = 0;
    [SerializeField] private Movement movement;
    [SerializeField] private Rigidbody character;
    [SerializeField] private GameObject firePoint1;
    [SerializeField] private GameObject firePoint2;
    [SerializeField] private Level3BoltScaling level3Beam;

    public List<CastedVFX> effects = new();
   
    private void Start()
    {        
        animator = GetComponent<Animator>();
        movement = GetComponentInParent<Movement>();
        character = GetComponentInParent<Rigidbody>();
    }

    // Cast Level
    public void SetCastLevel(int level)
    {   
        latestCastLevel = level;
        for(int i = minCastLevel; i <= maxCastLevel; i++)
        {
            animator.SetBool($"Level{i}", i <= level);       
            SetVFXBool($"Level {i} Available", i <= level);
        }        
    }

    // Animations
    public void TriggerAnimations()
    {
        SetVFXEnabled(true);
        animator.SetTrigger("MagicBolt");   
    }

    // Charge
    public void BeginCharge()
    {
        SetVFXEnabled(true);
    }

    // Casting
    public void VFXCast()
    {
        SetVFXTrigger("Cast");
    }

    public void Cast()
    {
        animator.SetTrigger("CastRelease");
        SetVFXTrigger("Cast");
    }

    public void EndCast()
    {
        SetVFXEnabled(false);
        StartMovement();
    }

    // Movement
    public void StopMovement()
    {
        movement.enabled = false;
        character.velocity = new Vector3(0,0,0);
    }

    public void StartMovement()
    {
        movement.enabled = true;
    }

    // Fire Point
    public void VFXFirePoint2()
    {
        SetVFXParent(firePoint2.transform, firePoint2.transform.position);
        //vfx.transform.Rotate(firePoint1.transform.rotation.x, firePoint1.transform.rotation.y, firePoint1.transform.rotation.z + 90);
    }

    public void VFXFirePoint1()
    {
        SetVFXParent(firePoint1.transform, new Vector3(0f,0f,0f));
    }

    // Beam
    public void FireLevel3Beam()
    {
        if (level3Beam != null)
        {
            level3Beam.gameObject.SetActive(true);
            level3Beam.cast = true;
        }
        else Debug.LogWarning("No Level 3 Beam to Fire.");
    }

    public void EndBeam()
    {
        if (level3Beam != null)
        {
            level3Beam.gameObject.SetActive(false);
        }
        else Debug.LogWarning("No Level 3 Beam to End.");
    }


    // VFX Helpers
    public void AddVFX(CastedVFX vfx)
    {
        if (vfx != null)
        {
            effects.Add(vfx);
            vfx.gameObject.SetActive(false);
            vfx.animator.enabled = false;
            vfx.VisualsEnabled = false;
            vfx.onSetPowerLimit.AddListener(SetCastLevel);
            vfx.onTrigger.AddListener(TriggerAnimations);
            vfx.onCast.AddListener((Vector3 v) => { Cast(); });
        }
    }

    public void SetVFXBool(string property, bool value)
    {
        foreach (var effect in effects)
        {
            if (effect.equipped)
                effect.animator.SetBool(property, value);
        }
    }

    public void SetVFXTrigger(string property)
    {
        foreach (var effect in effects)
        {
            if (effect.equipped)
                effect.animator.SetTrigger(property);
        }
    }

    public void SetVFXEnabled(bool enabled)
    {
        foreach (var effect in effects)
        {
            effect.gameObject.SetActive(enabled && effect.equipped);
            effect.animator.enabled = enabled && effect.equipped;
            effect.VisualsEnabled = enabled && effect.equipped;
        }
    }

    public void SetVFXParent(Transform parent, Vector3 position = new())
    {
        foreach (var effect in effects)
        {
            effect.transform.parent = parent;
            effect.transform.position = position;
        }
    }
}
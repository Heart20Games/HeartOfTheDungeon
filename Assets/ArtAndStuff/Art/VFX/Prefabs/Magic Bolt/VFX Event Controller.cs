using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VFXEventController : MonoBehaviour
{

    [SerializeField] private Animator vfxAnimator;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject vfx;



    private void Start()
    {
        vfxAnimator = this.transform.Find("MB Cast Point").GetComponent<Animator>();
        animator = this.GetComponent<Animator>();
        vfx = GameObject.Find("MB Cast Point");
        vfx.SetActive(false);
    }

    public void MagicBoltLevel1()
    {
        animator.SetBool("Level2", false);
        animator.SetBool("Level3", false);
        vfxAnimator.SetBool("Level 2 Available", false);
        vfxAnimator.SetBool("Level 3 Available", false);
        animator.SetTrigger("MagicBolt");
        vfx.SetActive(true);
    }

    public void MagicBoltLevel2()
    {
        animator.SetBool("Level2", true);
        animator.SetBool("Level3", false);
        vfxAnimator.SetBool("Level 2 Available", true);
        vfxAnimator.SetBool("Level 3 Available", false);
        animator.SetTrigger("MagicBolt");
        vfx.SetActive(true);
    }

    public void MagicBoltLevel3()
    {
        animator.SetBool("Level2", true);
        animator.SetBool("Level3", true);
        vfxAnimator.SetBool("Level 2 Available", true);
        vfxAnimator.SetBool("Level 3 Available", true);
        animator.SetTrigger("MagicBolt");
        vfx.SetActive(true);
    }

    public void BeginCharge()
    {
        vfx.SetActive(true);
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
        vfx.SetActive(false);
    }

}
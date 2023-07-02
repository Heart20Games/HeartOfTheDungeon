using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ExplodingBarrel : MonoBehaviour
{
    [SerializeField] private GameObject barrel;
    [SerializeField] public ParticleSystem explosion;
    [SerializeField] private BoxCollider barrelCollider;
    public bool exploded = false;
    public bool explode = false;



    
    // Start is called before the first frame update
    void Start()
    {
        barrelCollider = GetComponent<BoxCollider>();
    }

    private void Update() 
    {
         if(explode == true) 
         {
           explosion.Play(true);
           explode = false; 
         }  
    }


    private void OnTriggerEnter(Collider other) 
    {
        if(!exploded && other.tag == "Projectile")
        {
            explosion.Play(true);
            exploded = true;
        }    
    }
    
    
}

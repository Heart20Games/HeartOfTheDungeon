using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimation : MonoBehaviour
{
  
    private Animator anim;
    private float randomOffset;
    [SerializeField] private string animationName;

  
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        randomOffset = Random.Range(0f, 5f);

        anim.Play(animationName, 0, randomOffset);
        
    }

  
}

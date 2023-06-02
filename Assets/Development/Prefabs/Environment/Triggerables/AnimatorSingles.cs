using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSingles : BaseMonoBehaviour
{
    public Animator animator;
    public string boolName = "flying";

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void SetBool(bool value)
    {
        animator.SetBool(boolName, value);
    }
}

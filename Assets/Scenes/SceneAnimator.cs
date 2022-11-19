using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SceneAnimator : MonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    [YarnCommand("trigger")]
    public void Trigger(string key)
    {
        animator.SetTrigger(key);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SceneAnimator : BaseMonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        Debug.Log("SceneAnimator Started");
        animator = GetComponent<Animator>();
    }

    [YarnCommand("trigger")]
    public void Trigger(string key)
    {
        Debug.Log("Scene Animator Triggered");
        animator.SetTrigger(key);
    }
}

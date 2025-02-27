using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

public class ObjectAnimationSounds : BaseMonoBehaviour
{
    [field: SerializeField] public EventReference objectMoveOne { get; private set; }

    public static ObjectAnimationSounds instance { get; private set; }
    public void ObjectMoveOne(EventReference objectMoveOne, Vector3 position) 
    {
        RuntimeManager.PlayOneShot(objectMoveOne, transform.position);
    }
}

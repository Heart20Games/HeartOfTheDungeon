using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Yarn.Unity;
using FMOD.Studio;

public class YarnFMOD : BaseMonoBehaviour
{
    public FModEventLibary fmodEventLibrary;
    private StudioEventEmitter emitter;

    public void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();
    }

    [YarnCommand("emit_sound")]
    public void EmitByURL(string soundName)
    {
        emitter.Stop();
        emitter.EventReference = fmodEventLibrary.GetReference(soundName);
        emitter.Play();
    }

    [YarnCommand("stop_sound")]
    public void StopEmit()
    {
        emitter.Stop();
    }
}

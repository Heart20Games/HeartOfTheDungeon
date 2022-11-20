using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Yarn.Unity;

public class YarnFMOD : MonoBehaviour
{
    private StudioEventEmitter emitter;
    public void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();
    }

    [YarnCommand("urlEmit")]
    public void emitByURL(string url)
    {
        emitter.Stop();
        emitter.EventReference = EventReference.Find(url);
        emitter.Play();
    }

    [YarnCommand("stopEmit")]
    public void stopEmit()
    {
        emitter.Stop();
    }
}

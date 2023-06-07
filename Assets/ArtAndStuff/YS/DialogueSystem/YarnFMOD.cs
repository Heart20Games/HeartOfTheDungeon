using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Yarn.Unity;

public class YarnFMOD : BaseMonoBehaviour
{
    private StudioEventEmitter emitter;
    public void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();
    }

    [YarnCommand("url_emit")]
    public void emitByURL(string url)
    {
        emitter.Stop();
        emitter.EventReference = EventReference.Find(url);
        emitter.Play();
    }

    [YarnCommand("stop_emit")]
    public void stopEmit()
    {
        emitter.Stop();
    }
}

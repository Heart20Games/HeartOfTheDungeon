using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FModEventPlayer : BaseMonoBehaviour
{
    public FModEventLibary libary;
    public Vector3 offset = Vector3.zero;
    public Transform source;

    private void Awake()
    {
        if (source == null)
            source = gameObject.transform;
    }

    public void HelloWorld(string key)
    {
        Debug.Log("Hello world!");
    }

    public void PlayEvent(string key)
    {
        PlayEvent(key, source.gameObject);
    }

    public void PlayEvent(string key, GameObject obj)
    {
        if (libary == null)
        {
            Debug.LogWarning("Can't find any FMod Event Libary!");
        }
        else
        {
            EventReference reference = libary.GetReference(key);
            PlayOneShot(reference, obj);
        }
    }

    public void PlayOneShot(EventReference sound, GameObject obj)
    {
        RuntimeManager.PlayOneShotAttached(sound, obj);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }
}

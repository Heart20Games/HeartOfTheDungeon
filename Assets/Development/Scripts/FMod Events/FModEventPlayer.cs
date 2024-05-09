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
        Vector3 worldPos = source.position + offset;
        PlayEvent(key, worldPos);
    }

    public void PlayEvent(string key, Vector3 worldPos)
    {
        if (libary == null)
        {
            Debug.LogWarning("Can't find any FMod Event Libary!");
        }
        else
        {
            EventReference reference = libary.GetReference(key);
            PlayOneShot(reference, worldPos);
        }
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }
}

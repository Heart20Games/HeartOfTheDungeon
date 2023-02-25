using System.Collections;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using FMODUnity;

public class AudioView : DialogueViewBase
{
    public string skipTag = "no-va";
    public EventReference fmodEvent;
    public EventReference skipFmodEvent;
    private FMOD.Studio.EventInstance eventInstance;
    private FMOD.Studio.EventInstance skipInstance;

    //Action advanceHandler = null;

    public void Awake()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);
        eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        skipInstance = RuntimeManager.CreateInstance(skipFmodEvent);
        skipInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
    }

    private bool HasTag(string[] metaData, string tag)
    {
        if (metaData != null)
        {
            foreach (string meta in metaData)
            {
                if (meta == tag)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private void PlaySound(LocalizedLine dialogueLine)
    {
        bool skip = HasTag(dialogueLine.Metadata, skipTag);
        FMOD.Studio.EventInstance instance = skip ? skipInstance : eventInstance;
        FMOD.Studio.EventInstance other = skip ? eventInstance : skipInstance;

        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        other.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        string clean = dialogueLine.TextID.Remove(4, 1);
        if (!skip)
        {
            instance.setParameterByNameWithLabel("IntroVOParam", clean); // IntroVOParam is on the Intro Scene even (Fmod event)
        }
        instance.start();
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (gameObject.activeInHierarchy == false)
        {
            onDialogueLineFinished();
            return;
        }

        PlaySound(dialogueLine);
        onDialogueLineFinished();
        //base.RunLine(dialogueLine, onDialogueLineFinished);
    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        PlaySound(dialogueLine);
        onDialogueLineFinished();
        //base.InterruptLine(dialogueLine, onDialogueLineFinished);
    }

    public override void DismissLine(Action onDismissalComplete)
    {
        if (gameObject.activeInHierarchy == false)
        {
            onDismissalComplete();
            return;
        }

        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //skipInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        onDismissalComplete();
        //base.DismissLine(onDismissalComplete);
    }
}

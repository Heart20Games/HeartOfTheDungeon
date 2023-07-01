using Yarn.Unity;
using System;
using FMODUnity;
using static YarnTags;
using System.Collections.Generic;

public class AudioView : DialogueViewBase, IViewable
{
    public string paramSuffix = "_param";
    public string skipTag = "no-va";
    public string paramTag = "param";
    public string defaultParamTag = "Prologue-VO";
    public readonly ViewType viewType = ViewType.Audio;
    public FModEventLibary fmodEventLibrary;
    public EventReference fmodEvent;
    public EventReference skipFmodEvent;
    private FMOD.Studio.EventInstance eventInstance;
    private FMOD.Studio.EventInstance skipInstance;
    private Inclusion viewable = Inclusion.NA;
    private readonly Dictionary<EventReference, FMOD.Studio.EventInstance> eventInstances = new();

    //Action advanceHandler = null;

    public void Awake()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
        skipInstance = RuntimeManager.CreateInstance(skipFmodEvent);
        skipInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
    }

    public FMOD.Studio.EventInstance EventInstance(EventReference eventRef)
    {
        if (!eventInstances.TryGetValue(eventRef, out eventInstance))
        {
            eventInstance = RuntimeManager.CreateInstance(eventRef);
            eventInstances.Add(eventRef, eventInstance);
        }
        return eventInstance;
    }

    private void PlaySound(LocalizedLine dialogueLine)
    {
        bool skip = HasTag(dialogueLine.Metadata, skipTag);
        if (skip)
        {
            skipInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            skipInstance.start();
        }
        else
        {
            HasPairTag(dialogueLine.Metadata, paramTag, out string param, defaultParamTag);
            
            EventReference fmodParamEvent = fmodEventLibrary.GetReference(param);
            FMOD.Studio.EventInstance other = eventInstance;
            FMOD.Studio.EventInstance instance = EventInstance(fmodParamEvent);
            
            //string paramName = fmodParamEvent.Path[(fmodParamEvent.Path.LastIndexOf('/') + 1)..];
            string clean = dialogueLine.TextID.Remove(4, 1);
            instance.setParameterByNameWithLabel(param + paramSuffix, clean);//paramName, clean);

            other.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.start();
        }
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (gameObject.activeInHierarchy == false || !ShouldIncludeView(dialogueLine.Metadata, viewType, viewable))
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

    public void SetViewable(Inclusion viewable)
    {
        this.viewable = viewable;
    }

    public ViewType GetViewType()
    {
        return viewType;
    }
}

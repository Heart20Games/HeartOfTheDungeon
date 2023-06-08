using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using static YarnTags;

public class BubbleView : DialogueViewBase, IViewable
{
    public readonly ViewType viewType = ViewType.Bubble;
    private Inclusion viewable = Inclusion.NA;

    public ViewType GetViewType()
    {
        return viewType;
    }
    
    public void SetViewable(Inclusion viewable)
    {
        this.viewable = viewable;
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (gameObject.activeInHierarchy == false || !ShouldIncludeView(dialogueLine.Metadata, viewType, viewable))
        {
            onDialogueLineFinished();
            return;
        }

        onDialogueLineFinished();
        //base.RunLine(dialogueLine, onDialogueLineFinished);
    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
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

        onDismissalComplete();
        //base.DismissLine(onDismissalComplete);
    }
}

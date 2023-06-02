using System;
using Yarn.Unity;
using static YarnTags;

public class TaggedOptionsListView : OptionsListView
{
    public readonly ViewType viewType = ViewType.OptionList;

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (!Included(dialogueLine.Metadata, viewType))
        {
            onDialogueLineFinished();
            return;
        }

        base.RunLine(dialogueLine, onDialogueLineFinished);
    }
}

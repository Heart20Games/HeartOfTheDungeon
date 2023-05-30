using System;
using Yarn.Unity;
using static YarnTags;

public class TaggedLineView : LineView
{
    public readonly ViewType viewType = ViewType.Line;

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

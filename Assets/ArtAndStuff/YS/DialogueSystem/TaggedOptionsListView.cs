using System;
using Yarn.Unity;
using static YarnTags;

public class TaggedOptionsListView : OptionsListView, IViewable
{
    public readonly ViewType viewType = ViewType.OptionList;
    private Inclusion viewable = Inclusion.NA;

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (!ShouldIncludeView(dialogueLine.Metadata, viewType, viewable))
        {
            onDialogueLineFinished();
            return;
        }

        base.RunLine(dialogueLine, onDialogueLineFinished);
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

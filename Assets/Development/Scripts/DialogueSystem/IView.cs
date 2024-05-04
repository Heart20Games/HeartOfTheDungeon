using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static YarnTags;

public interface IViewable
{
    public void SetViewable(Inclusion viewable);
    public ViewType GetViewType();
}

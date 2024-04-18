using HotD.Castables;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CastCoordinator : MecanimCoordinator
{
    public UnityEvent<CastAction> onAction;

    public void RegisterActionListener(UnityAction<CastAction> listener, bool add = true)
    {
        if (add)
            onAction.AddListener(listener);
        else
            onAction.RemoveListener(listener);
    }

    public void SetAction(int idx)
    {
        SetInt("Action", idx);
        SetTrigger("StartAction");
    }

    [ButtonMethod]
    public void Cast()
    {
        StartCast();
        EndCast();
    }

    [ButtonMethod]
    public void StartCast()
    {
        onAction.Invoke(CastAction.Start);
    }

    [ButtonMethod]
    public void EndCast()
    {
        onAction.Invoke(CastAction.End);
    }
}

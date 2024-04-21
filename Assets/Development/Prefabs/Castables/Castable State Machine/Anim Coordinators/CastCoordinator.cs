using HotD.Castables;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CastCoordinator : MecanimCoordinator
{
    [Flags] public enum Triggers
    {
        None = 0,
        StartAction = 1 << 0,
        StartCast = 1 << 1,
    }

    public int ActionIndex { get => GetInt("Action"); set => SetActionIndex(value); }

    public UnityEvent<CastAction> onAction;

    public void RegisterActionListener(UnityAction<CastAction> listener, bool add = true)
    {
        if (add)
            onAction.AddListener(listener);
        else
            onAction.RemoveListener(listener);
    }

    private bool HasTrigger(Triggers triggers, Triggers trigger)
    {
        return (triggers & trigger) == trigger;
    }

    public void Coordinate(Triggers triggers)
    {
        if (HasTrigger(triggers, Triggers.StartAction))
        {
            SetTrigger("StartAction");
        }
        if (HasTrigger(triggers, Triggers.StartCast))
        {
            SetTrigger("StartCast");
        }
    }

    public void SetActionIndex(int idx, bool trigger=false)
    {
        SetInt("Action", idx);
        if (trigger)
            SetTrigger("StartAction");
    }

    public void OnEndCast()
    {
        Print("OnEndCast", debug);
        onAction.Invoke(CastAction.End);
    }

    [Header("Tests")]
    public bool debug = false;
    public int testIdx = 0;
    [ButtonMethod]
    public void TestSetAction()
    {
        SetActionIndex(testIdx);
    }

    [ButtonMethod]
    public void TestStartAction()
    {
        SetTrigger("StartAction");
    }

    [ButtonMethod]
    public void TestStartCast()
    {
        SetTrigger("StartCast");
    }
}

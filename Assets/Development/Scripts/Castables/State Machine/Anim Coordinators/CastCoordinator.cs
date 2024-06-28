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

    public int PowerLevel { get => GetInt("ChargeLevel"); set => SetPowerLevel(value); }
    public int ComboLevel { get => GetInt("ComboLevel"); set => SetComboLevel(value); }
    public ActionType ActionIndex { get => (ActionType)(int)GetFloat("Action"); set => SetActionIndex((int)value); }

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
        if (triggers == Triggers.None) return;
        if (HasTrigger(triggers, Triggers.StartAction))
        {
            Print($"Triggering StartAction ({triggers})", debugTriggers, this);
            SetTrigger("StartAction");
        }
        if (HasTrigger(triggers, Triggers.StartCast))
        {
            Print($"Triggering StartCast ({triggers})", debugTriggers, this);
            SetTrigger("StartCast");
        }
    }

    public void SetComboLevel(int level)
    {
        Print("Set Combo Level", debugValues, this);
        SetInt("ComboLevel", level);
    }

    public void SetPowerLevel(int level)
    {
        Print("Set Power Level", debugValues, this);
        SetInt("ChargeLevel", level);
    }

    public void SetActionIndex(int idx)
    {
        Print("Set Action Index", debugValues, this);
        SetFloat("Action", idx);
    }

    public void OnStartCast()
    {
        Print("OnStartCast", debugTriggers, this);
        onAction.Invoke(CastAction.End);
    }

    public void OnEndCast()
    {
        Print("OnEndCast", debugTriggers, this);
        onAction.Invoke(CastAction.End);
    }

    [Header("Tests")]
    public bool debug = false;
    [SerializeField] protected bool debugValues = false;
    [SerializeField] protected bool debugTriggers = false;
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

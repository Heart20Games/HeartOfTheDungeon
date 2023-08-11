using Cinemachine;
using CustomUnityEvents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ISelectable;


[Serializable]
public struct SelectorEvent
{
    public BinaryEvent trigger;
    public BinaryEvent<ASelectable> selectable;
    public void InvokeEnter(ASelectable selectable)
    {
        this.trigger.enter.Invoke();
        this.selectable.enter.Invoke(selectable);
    }
    public void InvokeExit(ASelectable selectable)
    {
        this.trigger.exit.Invoke();
        this.selectable.exit.Invoke(selectable);
    }
}


public class Selector : BaseMonoBehaviour
{
    [Header("Selection")]
    [SerializeField] private List<SelectType> selectableTypes;
    public List<SelectType> SelectableTypes { get { return selectableTypes; } set { SetSelectableTypes(value); } }
    public List<ASelectable> hoveringOver = new();
    public ASelectable selected;
    public SelectorEvent onConfirm;
    public SelectorEvent onSelect;
    public SelectorEvent onHover;
    public UnityEvent<List<SelectType>> onSetTypes;
    

    public void SetSelectableTypes(List<SelectType> types)
    {
        selectableTypes = types;
        onSetTypes.Invoke(selectableTypes);
    }


    // Selectables

    public virtual void Clear()
    {
        hoveringOver.Clear();
        DeSelect();
    }

    public virtual void Hover(ASelectable selectable)
    {
        if (selectable != null)
        {
            if (hoveringOver.Count > 0)
            {
                hoveringOver[^1].UnHover();
            }
            hoveringOver.Add(selectable);
            selectable.Hover();
            onHover.InvokeEnter(selectable);
        }
    }

    public virtual void UnHover(ASelectable selectable)
    {
        if (selectable != null)
        {
            if (selected = selectable)
            {
                DeSelect();
            }
            hoveringOver.Remove(selectable);
            selectable.UnHover();
            onHover.InvokeExit(selectable);
            if (hoveringOver.Count > 0)
            {
                hoveringOver[^1].Hover();
            }
        }
    }

    public virtual void Select()
    {
        if (hoveringOver.Count > 0)
        {
            selected = hoveringOver[^1];
            if (selected.isSelected)
            {
                Confirm();
            }
            else
            {
                selected.Select();
                onSelect.InvokeEnter(selected);
            }
        }
    }

    public virtual void DeSelect()
    {
        if (selected != null)
        {
            selected.DeSelect();
            onSelect.InvokeExit(selected);
            selected = null;
        }
    }

    public virtual void Confirm()
    {
        selected.Confirm();
        onConfirm.InvokeEnter(selected);
    }
}
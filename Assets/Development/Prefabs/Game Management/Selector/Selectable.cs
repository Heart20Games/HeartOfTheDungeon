using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ISelectable;

public class Selectable : ASelectable
{
    public BinaryEvent onSelection;
    public BinaryEvent onHover;
    public UnityEvent onConfirm;

    [SerializeField] private SelectType selectType = SelectType.Default;
    public override SelectType Type { get => selectType; set => selectType = value; }

    public override void Select()
    {
        base.Select();
        onSelection.enter.Invoke();
    }

    public override void DeSelect()
    {
        base.DeSelect();
        onSelection.exit.Invoke();
    }

    public override void Hover()
    {
        base.Hover();
        onHover.enter.Invoke();
    }

    public override void UnHover()
    {
        base.UnHover();
        onHover.exit.Invoke();
    }

    public override void Confirm()
    {
        onConfirm.Invoke();
    }
}

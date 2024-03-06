using CustomUnityEvents;
using UnityEngine;
using UnityEngine.Events;
using static ISelectable;

public class Selectable : ASelectable
{
    [SerializeField] private SelectType selectType = SelectType.Default;

    [Header("Events")]
    public BinaryEvent onSelection;
    public BinaryEvent onHover;
    public UnityEvent onConfirm;

    public override SelectType Type { get => selectType; set => selectType = value; }

    public void OnEnable()
    {
        DeSelect();
    }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ISelectable;

public abstract class ASelectable : BaseMonoBehaviour, ISelectable
{
    public abstract SelectType Type { get; set; }
    public void NoOpSelectType(SelectType value) { }
    public GameObject source;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public bool isHovering = false;

    [ReadOnly] public ASelectable next;
    [ReadOnly] public ASelectable last;
    [ReadOnly] public bool visible = false;

    private void Awake()
    {
        if (source == null)
        {
            source = gameObject;
        }
    }

    public virtual void Select()
    {
        isSelected = true;
    }

    public virtual void DeSelect()
    {
        isSelected = false;
    }

    public virtual void Hover()
    {
        isHovering = true;
    }

    public virtual void UnHover()
    {
        isHovering = false;
    }

    public abstract void Confirm();

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ISelectable;

public abstract class ASelectable : MonoBehaviour, ISelectable
{
    public abstract SelectType Type { get; set; }
    public void NoOpSelectType(SelectType value) { }
    public GameObject source;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public bool isHovering = false;

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
}

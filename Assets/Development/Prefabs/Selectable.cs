using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour, ISelectable
{
    public BinaryEvent onSelection;
    public BinaryEvent onHover;
    public UnityEvent onConfirm;
    public enum SelectType { Default, Triggerable, Character, Interactable , Disabled, Invalid }
    public SelectType type = SelectType.Default;

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
        onSelection.enter.Invoke();
    }

    public virtual void DeSelect()
    {
        isSelected = false;
        onSelection.exit.Invoke();
    }

    public virtual void Hover()
    {
        isHovering = true;
        onHover.enter.Invoke();
    }

    public virtual void UnHover()
    {
        isHovering = false;
        onHover.exit.Invoke();
    }

    public virtual void Confirm()
    {
        onConfirm.Invoke();
    }
}

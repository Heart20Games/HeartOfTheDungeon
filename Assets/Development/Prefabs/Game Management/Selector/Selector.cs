using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ISelectable;

public class Selector : BaseMonoBehaviour
{
    [Header("Selection")]
    [SerializeField] private List<SelectType> selectableTypes;
    public List<SelectType> SelectableTypes { get { return selectableTypes; } set { SetSelectableTypes(value); } }
    public List<ASelectable> hoveringOver = new();
    public ASelectable selected;
    public UnityEvent onConfirm;
    public UnityEvent<List<SelectType>> onSetTypes;
    

    public void SetSelectableTypes(List<SelectType> types)
    {
        selectableTypes = types;
        onSetTypes.Invoke(selectableTypes);
    }


    // Selectables

    public void Clear()
    {
        hoveringOver.Clear();
        DeSelect();
    }

    public void Hover(ASelectable selectable)
    {
        if (selectable != null)
        {
            if (hoveringOver.Count > 0)
            {
                hoveringOver[^1].UnHover();
            }
            hoveringOver.Add(selectable);
            selectable.Hover();
        }
    }

    public void UnHover(ASelectable selectable)
    {
        if (selectable != null)
        {
            if (selected = selectable)
            {
                DeSelect();
            }
            hoveringOver.Remove(selectable);
            selectable.UnHover();
            if (hoveringOver.Count > 0)
            {
                hoveringOver[^1].Hover();
            }
        }
    }

    public void Select()
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
            }
        }
    }

    public void DeSelect()
    {
        if (selected != null)
        {
            selected.DeSelect();
            selected = null;
        }
    }

    public void Confirm()
    {
        selected.Confirm();
        onConfirm.Invoke();
    }
}
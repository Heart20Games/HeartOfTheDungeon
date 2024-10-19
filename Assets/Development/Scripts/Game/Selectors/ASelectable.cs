using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;
using static ISelectable;

public abstract class ASelectable : BaseMonoBehaviour, ISelectable
{
    public abstract SelectType Type { get; set; }
    public void NoOpSelectType(SelectType value) { }
    public GameObject source;
    public Identity Identity { get => !source.TryGetComponent<AIdentifiable>(out var idable) ? Identity.Neutral : idable.Identity; }
    public Vector3 offset = Vector3.up;
    [ReadOnly] public bool isSelected = false;
    [ReadOnly] public bool isHovering = false;


    [ReadOnly] public ASelectable next;
    [ReadOnly] public ASelectable last;
    [ReadOnly] public bool visible = false;

    [SerializeField] private bool isSelectable = false;
    public UnityEvent<bool> onIsSelectable;
    public bool IsSelectable
    {
        get => isSelectable;
        set
        {
            isSelectable = value;
            onIsSelectable.Invoke(value);
        }
    }

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

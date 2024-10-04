using HotD.Castables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static HotD.Castables.CastableFields;

public class CastableStub : BaseMonoBehaviour, ICastable
{
    private CastableItem item;
    private Vector3 direction;

    public Vector3 Direction { get => direction; set => direction = value; }
    public bool CanCast => true;
    public CastableItem Item { get => item; set => item = value; }
    public Damager Damager => null;

    public void InitializeFields(CastableFieldsEditor field) { }
    public void Initialize(ICastCompatible owner, CastableItem item) { }
    public void QueueAction(CastAction action) { }
    public void SetActive(bool active) { }

    public FieldEvents FieldEvents { get => new(); }
    public float PowerLevel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int MaxPowerLevel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int ComboStep { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int MaxComboStep { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}

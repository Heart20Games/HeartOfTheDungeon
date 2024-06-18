using HotD.Castables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastableStub : BaseMonoBehaviour, ICastable
{
    private CastableItem item;
    private Vector3 direction;

    public Vector3 Direction { get => direction; set => direction = value; }
    public bool CanCast => true;
    public CastableItem Item { get => item; set => item = value; }
    public Damager Damager => null;

    public void Initialize(CastableFields field) { }
    public void Initialize(ICastCompatible owner, CastableItem item, int actionIndex = 0) { }
    public void QueueAction(CastAction action) { }
    public void SetActive(bool active) { }
}

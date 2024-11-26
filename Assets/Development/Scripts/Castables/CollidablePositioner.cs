using Body.Behavior.ContextSteering;
using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Colliders;

public class CollidablePositioner : Positionable, IChangeCollisionExceptions, IDamager
{
    [Foldout("Collision", true)]
    private Collider[] colliders;
    private Collider[] Colliders { get { return colliders ?? InitializeColliders(gameObject, out colliders, ref collidableObjects); } }
    [Foldout("Collision")] public List<GameObject> collidableObjects;

    [Foldout("Damage")] public UnityEvent<Impactor> hitDamageable;
    [Foldout("Damage")] public UnityEvent<Impactor> leftDamageable;

    public void InitializeEvents()
    {
        hitDamageable ??= new();
        leftDamageable ??= new();
    }

    // ICollidable
    public void AddException(Collider exception)
    {
        ChangeException(Colliders, exception, true);
    }
    public void RemoveException(Collider exception)
    {
        ChangeException(Colliders, exception, false);
    }
    public void AddExceptions(Collider[] exceptions)
    {
        ChangeExceptions(Colliders, exceptions, true);
    }
    public void RemoveExceptions(Collider[] exceptions)
    {
        ChangeExceptions(Colliders, exceptions, false);
    }

    // IDamager
    public void HitDamageable(Impactor impactor)
    {
        throw new System.NotImplementedException();
    }

    public void LeftDamageable(Impactor impactor)
    {
        throw new System.NotImplementedException();
    }

    public void SetDamagePosition(Vector3 damagePosition)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int amount, CSIdentity.Identity id = CSIdentity.Identity.Neutral)
    {
        throw new System.NotImplementedException();
    }
}

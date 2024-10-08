using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISetCollisionExceptions
{
    public Collider[] Exceptions { get; set; }
    public void SetExceptions(Collider[] exceptions);
}

public interface IChangeCollisionExceptions
{
    public void AddExceptions(Collider[] exceptions);
    public void RemoveExceptions(Collider[] exceptions);
    public void AddException(Collider exception);
    public void RemoveException(Collider exception);
}

public interface ICollisionExceptions : ISetCollisionExceptions, IChangeCollisionExceptions { }

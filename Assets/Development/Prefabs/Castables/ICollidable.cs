using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidable
{
    public void AddExceptions(Collider[] exceptions);
    public void RemoveExceptions(Collider[] exceptions);
    public void AddException(Collider exception);
    public void RemoveException(Collider exception);
}
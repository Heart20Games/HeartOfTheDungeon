using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Colliders
{
    public static void ChangeExceptions(Collider[] colliders, Collider[] exceptions, bool ignore)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            for (int j = 0; j < exceptions.Length; j++)
            {
                Physics.IgnoreCollision(colliders[i], exceptions[j], ignore);
            }
        }
    }

    public static void ChangeException(Collider[] colliders, Collider exception, bool ignore)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            Physics.IgnoreCollision(colliders[i], exception, ignore);
        }
    }

    public static Collider[] InitializeColliders(GameObject gameObject, out Collider[] colliders, ref List<GameObject> collidableObjects)
    {
        List<Collider> colliderList = new();
        collidableObjects.Add(gameObject);
        for (int i = 0; i < collidableObjects.Count; i++)
        {
            if (collidableObjects[i] != null)
            {
                Collider[] components = collidableObjects[i].GetComponents<Collider>();
                if (components != null)
                {
                    colliderList.AddRange(components);
                }
            }
        }
        colliders = colliderList.ToArray();
        return colliders;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class ProjectileSpawner : Positionable
{
    public float lifeSpan;
    public Transform pivot;
    public Transform projectile;
    public bool followBody = false;

    private void Awake()
    {
        if (pivot != null)
        {
            if (projectile == null)
            {
                Pivot pivotType = pivot.GetComponent<Pivot>();
                if (pivotType != null)
                {
                    projectile = pivotType.body;
                }
            }
            // Projectile is a prefab
            if (projectile.gameObject.scene.name == null)
            {
                projectile = Instantiate(projectile, pivot);
            }
        }
    }

    public override void SetOrigin(Transform source, Transform target)
    {
        base.SetOrigin(source, target);
    }

    public void Spawn()
    {
        Spawn(new Vector3());
    }

    public void Spawn(Vector3 direction=new Vector3())
    {
        Transform pInstance = Instantiate(pivot, source);
        Transform bInstance = projectile;
        LaunchInstance(direction, pInstance.transform, bInstance);
        StartCoroutine(CleanupInstance(pInstance.transform, bInstance));
    }

    public void Activate(Vector3 direction)
    {
        LaunchInstance(direction, pivot.transform, projectile);
    }

    public void LaunchInstance(Vector3 direction, Transform pInstance, Transform bInstance)
    {
        if (!followBody)
        {
            pInstance.position = target.position + offset;
        }
        else
        {
            pInstance.localPosition = offset;
        }
        bInstance.localPosition = new Vector3();
        pInstance.gameObject.SetActive(true);
        Vector2 dir = new Vector2(direction.x, direction.z);
        Quaternion bRotation = bInstance.localRotation;
        pInstance.SetRotationWithVector(dir, rOffset);
        bInstance.localRotation = bRotation;
    }

    public IEnumerator CleanupInstance(Transform pInstance, Transform bInstance)
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(pInstance.gameObject);
    }
}

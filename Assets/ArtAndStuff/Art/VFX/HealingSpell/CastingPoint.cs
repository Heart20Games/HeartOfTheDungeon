using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CastingPoint : MonoBehaviour
{
    
    [SerializeField] private Transform castingPoint;
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private Transform localCastingPoint;
    [SerializeField] public Transform castingTarget;
    [SerializeField] private Transform localCastingTarget;

    

    

    
    void FixedUpdate()
    {               
        localCastingPoint.transform.position = castingPoint.transform.position;
        localCastingPoint.transform.eulerAngles = castingPoint.transform.eulerAngles;
        visualEffect.SetVector3("CastingPoint_position", localCastingPoint.localPosition);
        visualEffect.SetVector3("CastingPoint_angles", localCastingPoint.localEulerAngles);

        
        
        localCastingTarget.transform.position = castingTarget.transform.position;
        visualEffect.SetVector3("CastingTarget", localCastingTarget.localPosition);
    }
}

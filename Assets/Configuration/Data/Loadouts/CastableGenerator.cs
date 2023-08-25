using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCastableGenerator", menuName = "Loadouts/CastableGenerator", order = 1)]
public class CastableGenerator : ScriptableObject
{
    public enum TargetingMethod { TargetBased, LocationBased, DirectionBased }
    public enum ExecutionMethod { ColliderBased, ProjectileBased, SelectionBased }

    public GameObject rig;
    public CastableStats stats;
    public TargetingMethod targetingMethod;
    public ExecutionMethod executionMethod;

    public void GenerateCastable()
    {

    }
}

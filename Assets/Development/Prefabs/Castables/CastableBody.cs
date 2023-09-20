using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CastableBody : BaseMonoBehaviour
{
    [ReadOnly] public Castable castable;

    public float PowerLevel { get => castable.PowerLevel; }
    public Transform WeaponPivot { get => castable.source.weaponLocation; }

    public UnityEvent<float> onSetPowerLevel;
    public UnityEvent<int> onSetPowerLimit;
}

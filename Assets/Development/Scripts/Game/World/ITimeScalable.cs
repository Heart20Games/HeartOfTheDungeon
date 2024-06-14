using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimeScalable
{
    public float TimeScale { get; set; }
    public float SetTimeScale(float timeScale);
}

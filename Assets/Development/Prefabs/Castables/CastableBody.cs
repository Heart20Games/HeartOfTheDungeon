using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CastableBody : BaseMonoBehaviour
{
    // Power Level
    public UnityEvent<float> onSetPowerLevel;
    public void SetPowerLevel(float powerLevel)
    {
        onSetPowerLevel.Invoke(powerLevel);
    }
}

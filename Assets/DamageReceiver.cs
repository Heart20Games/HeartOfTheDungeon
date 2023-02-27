using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageReceiver : MonoBehaviour, IDamageable
{
    public UnityEvent<float> onTakeDamage;

    public void TakeDamage(float amount)
    {
        onTakeDamage.Invoke(amount);
    }
}

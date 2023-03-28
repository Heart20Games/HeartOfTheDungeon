using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageReceiver : MonoBehaviour, IDamageable
{
    public UnityEvent<int> onTakeDamage;

    public void TakeDamage(int amount)
    {
        onTakeDamage.Invoke(amount);
    }
}

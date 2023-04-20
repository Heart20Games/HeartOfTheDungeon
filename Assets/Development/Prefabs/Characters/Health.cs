using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour, IHealth
{
    public abstract void HealDamage(int amount);
    public abstract void SetHealth(int amount);
    public abstract void SetHealthBase(int amount, int total);
    public abstract void SetHealthTotal(int amount);
    public abstract void TakeDamage(int amount);
}

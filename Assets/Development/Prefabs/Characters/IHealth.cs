using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public void SetHealthTotal(int amount);
    public void SetHealthBase(int amount, int total);
    public void SetHealth(int amount);
    public void TakeDamage(int amount);
    public void HealDamage(int amount);
}

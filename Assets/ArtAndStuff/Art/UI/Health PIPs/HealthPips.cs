using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public class HealthPips : Health
{
    [SerializeField]
    private List<GameObject> healthPips = new();
    private readonly List<Animator> pipAnimator = new();
    [SerializeField]
    private GameObject healthPipPrefab;
    [SerializeField]
    private Transform healthPipCanvas;
    public int lastDamaged = -1;

    void Start()
    {
        SetHealthTotal(healthTotal);
    }

    private void FixedUpdate()
    {
        transform.TrueLookAt(Camera.main.transform.position);
    }

    private void ClearPips()
    {
        foreach (GameObject pip in healthPips)
        {
            Destroy(pip);
        }
        healthPips.Clear();
        pipAnimator.Clear();
    }

    public override void SetHealthBase(int amount, int total)
    {
        health = amount;
        SetHealthTotal(total);
    }

    public override void SetHealthTotal(int amount)
    {
        healthTotal = amount;
        ClearPips();
        for (int i = 0; i < healthTotal; i++)
        {
            GameObject pip = Instantiate(healthPipPrefab, healthPipCanvas);
            healthPips.Add(pip);
            pipAnimator.Add(pip.GetComponent<Animator>());
        }
        SetHealth(Mathf.Min(healthTotal, health));
    }

    public override void SetHealth(int amount)
    {
        health = amount;
        int damage = Mathf.Min(healthTotal - health, healthTotal);
        if (isActiveAndEnabled)
        {
            for (int i = 0; i < damage; i++)
            {
                pipAnimator[i].SetBool("IsDamaged", true);
                lastDamaged = i;
            }
            for (int i = damage; i < healthPips.Count; i++)
            {
                pipAnimator[i].SetBool("IsDamaged", false);
            }
        }
    }

    public override void TakeDamage(int amount, Identity id = Identity.Neutral)
    {
        int damageToTake = Mathf.Clamp(amount, 0, (healthTotal - (lastDamaged + 1)));
        for(int i = 0; i < damageToTake; i++)
        {                                               
            lastDamaged++;
            if (isActiveAndEnabled)
            {
                pipAnimator[lastDamaged].SetBool("IsDamaged", true);                                        
            }
        }
    }

    public override void HealDamage(int amount)
    {
        int damageToHeal = Mathf.Clamp(amount, 0, (lastDamaged + 1));
        for(int i = 0; i < damageToHeal; i++)
        {                                    
            if (isActiveAndEnabled)
            {
                pipAnimator[lastDamaged].SetBool("IsDamaged", false);
            }
            lastDamaged--;                                       
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPips : Health
{
    public int totalHealth;
    public int currentHealth;
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
        SetHealthTotal(totalHealth);
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
        currentHealth = amount;
        SetHealthTotal(total);
    }

    public override void SetHealthTotal(int amount)
    {
        totalHealth = amount;
        ClearPips();
        for (int i = 0; i < totalHealth; i++)
        {
            GameObject pip = Instantiate(healthPipPrefab, healthPipCanvas);
            healthPips.Add(pip);
            pipAnimator.Add(pip.GetComponent<Animator>());
        }
        SetHealth(Mathf.Min(totalHealth, currentHealth));
    }

    public override void SetHealth(int amount)
    {
        currentHealth = amount;
        int damage = totalHealth - currentHealth;
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

    public override void TakeDamage(int amount)
    {
        int damageToTake = Mathf.Clamp(amount, 0, (totalHealth - (lastDamaged + 1)));
            
        for(int i = 0; i < damageToTake; i++)
        {                                               
            lastDamaged++;
            pipAnimator[lastDamaged].SetBool("IsDamaged", true);                                        
        }
    }

    public override void HealDamage(int amount)
    {
        int damageToHeal = Mathf.Clamp(amount, 0, (lastDamaged + 1));

        for(int i = 0; i < damageToHeal; i++)
        {                                                    
            pipAnimator[lastDamaged].SetBool("IsDamaged", false);
            lastDamaged--;                                       
        }
    }
}

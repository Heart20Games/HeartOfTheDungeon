using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public class EnemyHealth : BaseMonoBehaviour, IDamageable
{
    public int startingHealth = 25;
    public int currentHealth;
    public HealthbarUI healthBarUI;

    void Awake()
    {
        currentHealth = startingHealth;
    }

    private void Start() 
    {
        healthBarUI.UpdateFill(currentHealth, startingHealth);    
    }

    private void Update() 
    {
        healthBarUI.UpdateFill(currentHealth, startingHealth);      
    }

    public void TakeDamage(int damageAmount, Identity id=Identity.Neutral)
    {
        currentHealth -= damageAmount;
        healthBarUI.UpdateFill(currentHealth, startingHealth);
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle death logic here, such as playing a death animation or removing the GameObject from the scene.
        Destroy(gameObject);
    }
}

using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float startingHealth = 25f;
    public float currentHealth;
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

    public void TakeDamage(float damageAmount)
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

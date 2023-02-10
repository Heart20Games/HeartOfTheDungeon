using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float startingHealth = 25f;
    private float currentHealth;

    void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        
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

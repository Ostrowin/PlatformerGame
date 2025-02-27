using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
    private HealthBar healthBar;

    public float regenTime = 5f;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
            healthBar.Initialize(currentHealth);

        InvokeRepeating(nameof(RegenerateHealth), regenTime, regenTime);
    }

    void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            healthBar?.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar?.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log(name + " zginął!");
        Destroy(gameObject);
    }
}

using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;
    public HealthBar healthBar; // 🔥 Teraz przypisujemy ręcznie w Inspectorze!

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth);
        }
        else
        {
            Debug.LogError("❌ Brak przypisanego HealthBar w PlayerHealth!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 🔥 Zapobiega ujemnym wartościom HP

        Debug.Log($"🔥 Gracz otrzymał {damage} obrażeń! HP: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            Debug.LogError("❌ Nie można zaktualizować paska zdrowia - HealthBar jest NULL!");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("💀 Gracz zginął!");
        // Możesz dodać restart poziomu lub animację śmierci
    }
}

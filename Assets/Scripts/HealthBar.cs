using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarFill; // Pasek zdrowia (zielony)
    private float maxHealth;
    private float currentHealth;

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        currentHealth = maxHP;
        UpdateHealthBar();
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        Debug.Log("Nowe HP: " + currentHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
            Debug.Log("Nowa wartość fillAmount: " + healthBarFill.fillAmount);
        }
    }
}

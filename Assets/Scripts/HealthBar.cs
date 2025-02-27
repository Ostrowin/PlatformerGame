using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarFill;
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
        currentHealth = Mathf.Clamp(health, 0, maxHealth); // 🔥 Zapobiega błędom
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
        else
        {
            Debug.LogError("❌ HealthBarFill nie przypisany w Inspectorze!");
        }
    }
}

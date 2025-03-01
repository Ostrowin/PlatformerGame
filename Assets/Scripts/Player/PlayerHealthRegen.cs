using UnityEngine;
using System.Collections;

public class PlayerHealthRegen : MonoBehaviour
{
    public float regenTime = 3f;
    public int regenAmount = 1;
    private PlayerHealth playerHealth;
    private bool canRegenerate = true;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            enabled = false;
            return;
        }

        StartCoroutine(RegenerateHealth());
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenTime);

            if (canRegenerate && playerHealth.GetCurrentHealth() < playerHealth.maxHealth)
            {
                playerHealth.Heal(regenAmount);
            }
        }
    }

    public void EnableRegeneration(bool enable)
    {
        canRegenerate = enable;
    }
}

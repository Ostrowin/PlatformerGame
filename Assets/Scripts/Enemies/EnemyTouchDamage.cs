using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 1; // Ilość obrażeń zadawanych graczowi
    public float damageCooldown = 1f; // Czas między zadawaniem obrażeń
    private float lastDamageTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DealDamage(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time >= lastDamageTime + damageCooldown)
        {
            DealDamage(collision.gameObject);
        }
    }

    private void DealDamage(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            lastDamageTime = Time.time;
            Debug.Log($"👻 EnemyGhost zadał {damage} obrażeń graczowi!");
        }
    }
}

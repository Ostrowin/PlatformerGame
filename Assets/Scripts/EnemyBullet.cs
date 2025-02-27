using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    public int shootDamage = 1;
    private Vector2 direction;
    private Rigidbody2D rb;

    public void Initialize(Vector2 shootDirection)
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Brak Rigidbody2D w EnemyBullet!");
            return;
        }

        // 🔥 Normalizacja kierunku + lekkie podniesienie
        direction = (shootDirection + new Vector2(0, 0.2f)).normalized;

        // 🔥 Nadajemy prędkość pociskowi
        rb.velocity = direction * speed;

        // 🔥 Auto-destrukcja po 3s
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerScript = collision.GetComponent<PlayerHealth>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(shootDamage);
                Debug.Log("🔥 Gracz dostał obrażenia: " + shootDamage);
            }

            Destroy(gameObject); // 🔥 Pocisk znika po trafieniu
        }
        else if (collision.CompareTag("Ground")) // Opcjonalnie: Pocisk niszczy się po trafieniu w ścianę
        {
            Destroy(gameObject);
        } 
        else if (collision.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}

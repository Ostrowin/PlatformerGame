using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;
    public int damage = 1;
    public float gravityScale = 0f;
    public float lifespan = 3f;
    public bool canHitEnemies = true;
    public bool canHitPlayer = false;
    
    [Header("Shooting Offset")]
    public float shootAngleOffset = 0.1f;

    private Vector2 direction;
    private Rigidbody2D rb;

    public void Initialize(Vector2 shootDirection)
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("❌ Brak Rigidbody2D w BulletBase!");
            return;
        }

        // ✅ Ustawiamy detekcję kolizji na "Continuous" – zapobiega przelatywaniu przez cienkie obiekty
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // ✅ Pocisk nie powinien obracać się po uderzeniu w coś
        rb.freezeRotation = true;

        // 🔥 Dodajemy offset strzału w górę
        direction = (shootDirection + new Vector2(0, shootAngleOffset)).normalized;

        rb.gravityScale = gravityScale;
        rb.velocity = direction * speed;

        Debug.Log($"🚀 Pocisk wystrzelony! Kierunek: {direction}, Prędkość: {rb.velocity}");

        Destroy(gameObject, lifespan);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"🎯 Pocisk uderzył w {collision.gameObject.name}");

        if (collision.CompareTag("Player") && canHitPlayer)
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"🔥 Gracz otrzymał {damage} obrażeń!");
            }
            Destroy(gameObject);
        }

        if (collision.CompareTag("Enemy") && canHitEnemies)
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"💥 Pocisk trafił wroga i zadał {damage} obrażeń!");
            }
            Destroy(gameObject);
        }

        if (collision.CompareTag("Ground") || collision.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}

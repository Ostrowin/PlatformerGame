using UnityEngine;

public class StrongBullet : MonoBehaviour
{
    public float speed = 20f;
    public int shootDamage = 3;

    private Vector2 direction;

    public void Initialize(Vector2 shootDirection)
    {
        // 🔥 Strzał lekko w górę (kąt ~10 stopni)
        direction = (shootDirection + new Vector2(0, 0.2f)).normalized;

        Destroy(gameObject, 3f); // 🔥 Auto-destrukcja po 3s
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) return; // ❌ Nie niszcz pocisku, jeśli trafi gracza

        EnemyChaseAI enemyChase = collision.GetComponent<EnemyChaseAI>();
        EnemyAI enemyPatrol = collision.GetComponent<EnemyAI>();

        if (enemyChase != null || enemyPatrol != null)
        {
            Debug.Log($"💥 SILNY pocisk trafił przeciwnika: {collision.gameObject.name}");

            // 🔥 Silniejszy odrzut przeciwnika
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockback = new Vector2(direction.x * 6f, 3f);
                enemyRb.velocity = Vector2.zero;
                enemyRb.AddForce(knockback, ForceMode2D.Impulse);
            }

            // 🔥 Większe obrażenia
            if (enemyChase != null) enemyChase.TakeDamage(shootDamage);
            if (enemyPatrol != null) enemyPatrol.TakeDamage(shootDamage);

            Destroy(gameObject); // 🔥 Pocisk znika po trafieniu
        }
        else
        {
            Destroy(gameObject); // 🔥 Pocisk znika po uderzeniu w ścianę
        }
    }
}

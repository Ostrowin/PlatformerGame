using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int shootDamage = 1;

    private Vector2 direction;

    public void Initialize(Vector2 shootDirection)
    {
        // 🔥 Strzał lekko w górę (kąt ~10 stopni)
        direction = (shootDirection + new Vector2(0, 0.1f)).normalized;

        // 🔥 Włączamy fizykę opadania
        GetComponent<Rigidbody2D>().gravityScale = 0.3f;

        Destroy(gameObject, 3f); // 🔥 Auto-destrukcja po 3s
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return; // ❌ Nie usuwamy pocisku, jeśli trafi gracza

        if (collision.CompareTag("Enemy")) 
        {
            Debug.Log("💥 Pocisk trafił przeciwnika!");

            // 🔥 Sprawdź, czy przeciwnik ma skrypt zdrowia
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

            if (enemyHealth != null) enemyHealth.TakeDamage(shootDamage); 

            Destroy(gameObject); // 🔥 Pocisk znika po trafieniu
        }
        else
        {
            Destroy(gameObject); // 🔥 Pocisk znika po uderzeniu w ścianę
        }
    }
}

using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    private Vector2 direction;

    public void Initialize(Vector2 shootDirection)
    {
        // 🔥 Strzał lekko w górę (kąt ~10 stopni)
        direction = (shootDirection + new Vector2(0, 0.2f)).normalized;

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
        if (!collision.CompareTag("Player")) // Nie niszcz kuli, jeśli uderzy w gracza
        {
            Destroy(gameObject);
        }
    }
}

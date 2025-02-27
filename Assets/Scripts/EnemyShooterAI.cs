using UnityEngine;

public class EnemyShooterAI : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    private HealthBar healthBar;

    [Header("Enemy Stats")]
    public int maxHealth = 5; // 🔥 Ustaw HP przeciwnika
    private int currentHealth;
    public float regenTime = 5f; // hp regen

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float minDistance = 5f;
    public float detectionRange = 10f;
    
    [Header("Shooting Settings")]
    public float bulletSpeed = 10f;
    public float fireRate = 2f;

    private float nextFireTime;

    void Start()
    {
        currentHealth = maxHealth; // 🔥 Ustawiamy HP na start
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(currentHealth);
            // Debug.Log("Zainicjalizowano pasek HP dla: " + name + " z HP: " + currentHealth);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        InvokeRepeating(nameof(RegenerateHealth), regenTime, regenTime);
    }

    void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            // Debug.Log(name + " odzyskał 1 HP! Aktualne HP: " + currentHealth);

            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth); // Aktualizacja paska HP
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 🔥 Jeśli gracz jest poza zasięgiem, przeciwnik nic nie robi
        if (distance > detectionRange) return;

        // Przeciwnik goni gracza, ale trzyma dystans
        if (distance < minDistance)
        {
            MoveAwayFromPlayer();
        }
        else
        {
            ChasePlayer();
        }

        // Obracanie FirePoint w stronę gracza
        UpdateFirePoint();

        // Strzelanie co określony czas
        if (Time.time >= nextFireTime)
        {
            ShootAtPredictedPosition();
            nextFireTime = Time.time + fireRate;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Debug.Log($"🔥 EnemyShooter otrzymał {damage} obrażeń! HP: {currentHealth}");

        if (healthBar != null)
        {
            // Debug.Log("!!!Aktualizacja paska HP dla: " + name);
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            // Debug.LogWarning("Brak komponentu HealthBar dla: " + name);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💀 EnemyShooter ZGINĄŁ!");
        Destroy(gameObject); // 🔥 Przeciwnik znika po śmierci
    }

    void UpdateFirePoint()
    {
        if (player == null || firePoint == null) return;

        bool isPlayerOnRight = player.position.x > transform.position.x;
        float firePointOffset = 0.7f;

        // 🔥 PRZESUWAMY FIREPOINT WZGLĘDEM PRZECIWNIKA
        firePoint.localPosition = new Vector2(isPlayerOnRight ? firePointOffset : -firePointOffset, 0);

    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void MoveAwayFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void ShootAtPredictedPosition()
    {
        // Debug.Log("Shooter shoot");

        if (bulletPrefab == null)
        {
            // Debug.LogError("❌ Brak przypisanego `bulletPrefab` w EnemyShooterAI!");
            return;
        }

        if (firePoint == null)
        {
            // Debug.LogError("❌ Brak przypisanego `firePoint` w EnemyShooterAI!");
            return;
        }

        Vector2 playerVelocity = player.GetComponent<Rigidbody2D>().velocity;
        float timeToTarget = Vector2.Distance(transform.position, player.position) / bulletSpeed;
        Vector2 predictedPosition = (Vector2)player.position + playerVelocity * timeToTarget;

        Vector2 direction = (predictedPosition - (Vector2)firePoint.position).normalized;

        // Tworzenie nowego pocisku
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Debug.Log("✅ Stworzono pocisk: " + bullet.name + " w " + firePoint.position);

        // Upewniamy się, że pocisk ma skrypt EnemyBullet
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction);
        }
        else
        {
            Debug.LogError("❌ `EnemyBullet` NIE ZNALEZIONY na prefabie pocisku!");
        }
    }
}

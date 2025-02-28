using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    
    public float bulletSpeed = 10f;
    public float fireRate = 2f;
    public LayerMask groundLayer; // 🔥 Nowy LayerMask dla przeszkód!
    private float nextFireTime;

    void Update()
    {
        if (player == null) return;

        if (Time.time >= nextFireTime)
        {
            ShootAtPredictedPosition();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootAtPredictedPosition()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("❌ Brak bulletPrefab lub firePoint w EnemyShoot!");
            return;
        }

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        Vector2 playerVelocity = playerRb != null ? playerRb.velocity : Vector2.zero;

        // 🔥 Obliczamy czas dotarcia pocisku do gracza
        float timeToTarget = Vector2.Distance(firePoint.position, player.position) / bulletSpeed;

        // 🔥 Przewidujemy przyszłą pozycję gracza
        Vector2 predictedPosition = (Vector2)player.position + playerVelocity * timeToTarget;

        // 🔥 Sprawdzamy, czy pocisk trafi w ziemię
        bool willHitGround = Physics2D.Raycast(firePoint.position, predictedPosition - (Vector2)firePoint.position, 
                                               Vector2.Distance(firePoint.position, predictedPosition), groundLayer);

        // 🔥 Jeśli przewidywany strzał trafi w ziemię → celujemy poziomo
        if (willHitGround)
        {
            predictedPosition = (Vector2)player.position; // 🔥 Strzelamy w obecną pozycję gracza
        }
        else if (playerVelocity.y < -0.1f) 
        {
            // 🔥 Jeśli gracz SPADA → strzelamy poziomo zamiast w dół
            predictedPosition = new Vector2(player.position.x, firePoint.position.y);
        }

        // 🔥 Obliczamy kierunek strzału
        Vector2 shootDirection = (predictedPosition - (Vector2)firePoint.position).normalized;

        // 🔥 Tworzymy pocisk
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        BulletBase bulletScript = bullet.GetComponent<BulletBase>();

        if (bulletScript != null)
        {
            bulletScript.Initialize(shootDirection);
        }
        else
        {
            Debug.LogError("❌ `BulletBase` NIE ZNALEZIONY na prefabie pocisku!");
        }
    }
}

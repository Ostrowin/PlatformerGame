using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 10f;
    public float fireRate = 2f;
    private float nextFireTime;

    void Update()
    {
        if (player == null) return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();

        if (bulletScript != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            bulletScript.Initialize(direction);
        }
    }
}

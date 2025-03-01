using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject strongBulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 2f;
    public float strongShootCooldown = 8f;
    public Sprite shootIcon;
    public Sprite strongShootIcon;

    private CooldownSystem cooldownSystem;
    private CooldownUI cooldownUI;

    private KeyCombinationManager keyManager;
    private PlayerDirection playerDirection;

    private void Start()
    {
        cooldownSystem = FindObjectOfType<CooldownSystem>();
        cooldownUI = FindObjectOfType<CooldownUI>();
        keyManager = FindObjectOfType<KeyCombinationManager>();
        playerDirection = GetComponent<PlayerDirection>();

        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.E }, () => PerformShoot());
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.E }, () => PerformStrongShoot());
    }

    public void PerformShoot()
    {
        if (cooldownSystem.IsOnCooldown("Shoot")) return;

        cooldownSystem.StartCooldown("Shoot", shootCooldown);
        cooldownUI.AddCooldown("Shoot", shootCooldown, shootIcon);
        Shoot(bulletPrefab);
    }

    public void PerformStrongShoot()
    {
        if (cooldownSystem.IsOnCooldown("Strong Shoot")) return;

        cooldownSystem.StartCooldown("Strong Shoot", strongShootCooldown);
        cooldownUI.AddCooldown("Strong Shoot", strongShootCooldown, strongShootIcon);
        Shoot(strongBulletPrefab);
    }

    private void Shoot(GameObject bulletPrefab)
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector2 shootDirection = playerDirection.lastMoveDirection;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<BulletBase>().Initialize(shootDirection);
    }
}

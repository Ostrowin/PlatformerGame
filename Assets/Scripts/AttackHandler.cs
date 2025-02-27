using UnityEngine;
using System.Collections;

public class AttackHandler : MonoBehaviour
{
    public Transform attackPoint;
    public float strongAttackRange = 1f;
    public float weakAttackRange = 1.5f;
    public float attackForce = 7f;
    public LayerMask enemyLayers;
    
    public static bool isPerformingSpecialAttack = false;
    private Vector2 lastMoveDirection = Vector2.right; // Domyślnie patrzy w prawo
    
    private bool canBasicAttack = true;
    private bool canStrongAttack = true;
    private bool canWeakAttack = true;
    private bool canStrongShoot = true;
    private bool canShoot = true;

    public float basicAttackCooldown = 0.5f;
    public float strongAttackCooldown = 2f;
    public float weakAttackCooldown = 1f;
    public float strongShootCooldown = 8f;
    public float shootCooldown = 2f;

    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.2f;

    private CooldownManager cooldownManager;
    public Sprite strongAttackIcon; // Ikona dla silnego ataku
    public Sprite weakAttackIcon;   // Ikona dla szybkiego ataku
    public Sprite strongShootIcon;   // Ikona dla mocnego strzału
    public Sprite shootIcon;   // Ikona dla strzału

    public GameObject bulletPrefab; // 🔫 Prefab pocisku
    public GameObject strongBulletPrefab; // 🔫 Prefab pocisku
    private GameObject player;
    private Rigidbody2D rb;
    
    void Start()
    {
        var keyManager = FindObjectOfType<KeyCombinationManager>();
        cooldownManager = FindObjectOfType<CooldownManager>();
        player = GameObject.FindGameObjectWithTag("Player"); // 🔥 Szuka gracza w scenie
        rb = player.GetComponent<Rigidbody2D>();

        // 🔥 Silny atak w lewo i prawo
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.LeftArrow, KeyCode.E }, () => StrongAttack(Vector2.left));
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.RightArrow, KeyCode.E }, () => StrongAttack(Vector2.right));

        // 🔥 Silny atak w górę
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.UpArrow, KeyCode.E }, () => StrongAttack(Vector2.up));

        // 🔥 Słabszy atak o dalszym zasięgu w bok
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.LeftArrow, KeyCode.E }, () => StartCoroutine(WeakAttackWithDash(Vector2.left)));
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.RightArrow, KeyCode.E }, () => StartCoroutine(WeakAttackWithDash(Vector2.right)));

        // 🔥 Słabszy atak o dalszym zasięgu w górę
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.UpArrow, KeyCode.E }, () => WeakAttack(Vector2.up));
        
        // 🔥 Atak podstawowy
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.E }, () => BasicAttack());
        
        // 🔥 Strzał
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.E }, () => PerformShootAttack());

        // 🔥 Mocny strzał
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.E }, () => PerformStrongShoot());
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        if (move != 0)
        {
            lastMoveDirection = new Vector2(move, 0).normalized; // 🔥 Zapamiętujemy kierunek ruchu
        }
    }

    void StrongAttack(Vector2 direction)
    {
        if (isPerformingSpecialAttack || !canStrongAttack) return;

        isPerformingSpecialAttack = true;
        canStrongAttack = false;
        // Debug.Log($"Silny atak w kierunku {direction}");
        cooldownManager.StartCooldown("Strong Attack", strongAttackCooldown, strongAttackIcon);
        PerformAttack(direction, strongAttackRange, attackForce, Color.yellow, 3);

        StartCoroutine(ResetSpecialAttack());
        StartCoroutine(ResetAttackCooldown(nameof(canStrongAttack), strongAttackCooldown));
    }

    void WeakAttack(Vector2 direction)
    {
        // if (isPerformingSpecialAttack || !canWeakAttack) return;

        // isPerformingSpecialAttack = true;
        // canWeakAttack = false;
        // Debug.Log($"Słaby atak w kierunku {direction}");

        cooldownManager.StartCooldown("Weak Attack", weakAttackCooldown, weakAttackIcon);
        PerformAttack(direction, weakAttackRange, attackForce / 2, new Color(0.5f, 0f, 0.5f), 1);

        // StartCoroutine(ResetSpecialAttack());
        StartCoroutine(ResetAttackCooldown(nameof(canWeakAttack), weakAttackCooldown));
    }

    void BasicAttack()
    {
        if (isPerformingSpecialAttack || !canBasicAttack) return;

        canBasicAttack = false;
        // cooldownManager.StartCooldown("Basic Attack", basicAttackCooldown, basicAttackIcon);
        PerformAttack(lastMoveDirection, 1f, attackForce / 2, Color.white, 2);

        StartCoroutine(ResetAttackCooldown(nameof(canBasicAttack), basicAttackCooldown));
    }

    IEnumerator ResetSpecialAttack()
    {
        yield return new WaitForSeconds(0.3f); // 🔥 Czas, po którym `E` znowu działa
        isPerformingSpecialAttack = false;
    }

    IEnumerator ResetAttackCooldown(string attackType, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        if (attackType == nameof(canBasicAttack))
            canBasicAttack = true;
        else if (attackType == nameof(canStrongAttack))
            canStrongAttack = true;
        else if (attackType == nameof(canWeakAttack))
            canWeakAttack = true;
        else if (attackType == nameof(canStrongShoot))
            canStrongShoot = true;
        else if (attackType == nameof(canShoot))
            canShoot = true;

        // Debug.Log($"{attackType} gotowy do użycia!");
    }

    void PerformAttack(Vector2 direction, float range, float force, Color attackColor, int damage)
    {
        Vector3 attackPosition = transform.position + (Vector3)(direction * range);

        // 🔥 Tymczasowa wizualizacja ataku
        GameObject attackEffect = GameObject.CreatePrimitive(PrimitiveType.Quad);
        attackEffect.transform.position = attackPosition;
        attackEffect.transform.position += new Vector3(0, 0, -1); // 🔥 Wypycha do przodu
        attackEffect.transform.localScale = new Vector3(range, range, 1);

        Renderer attackRenderer = attackEffect.GetComponent<Renderer>();
        if (attackRenderer != null)
        {
            attackRenderer.sortingLayerName = "Foreground";
            attackRenderer.sortingOrder = 10;
            attackRenderer.material.color = attackColor;
        }

        Destroy(attackEffect, 0.1f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, range, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"Trafiono: {enemy.name} | Zadano {damage} obrażeń!");

            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null && enemyRb.bodyType == RigidbodyType2D.Dynamic)
            {
                Vector2 knockbackDirection = direction.normalized;
                Vector2 knockback = knockbackDirection * force;
                knockback.y = Mathf.Abs(knockback.y) + 2f;

                enemyRb.velocity = Vector2.zero;
                enemyRb.AddForce(knockback, ForceMode2D.Impulse);
            }

            EnemyChaseAI enemyChase = enemy.GetComponent<EnemyChaseAI>();
            EnemyAI enemyPatrol = enemy.GetComponent<EnemyAI>();
            EnemyShooterAI enemyShooter = enemy.GetComponent<EnemyShooterAI>();

            if (enemyChase != null)
            {
                enemyChase.TakeDamage(damage);
            }
            else if (enemyPatrol != null)
            {
                enemyPatrol.TakeDamage(damage);
            }
            else if (enemyShooter != null)
            {
                enemyShooter.TakeDamage(damage);
            }
        }
    }
    
    private void PerformShootAttack()
    {
        if (player == null)
        {
            Debug.LogError("❌ Brak referencji do gracza w AttackHandler!");
            return;
        }
        if (isPerformingSpecialAttack || !canShoot) return;
        canShoot = false;

        Debug.Log("🔫 Gracz strzela!");

        // 🔥 Używamy `lastMoveDirection`, aby określić stronę strzału
        Vector2 shootDirection = lastMoveDirection.x >= 0 ? Vector2.right : Vector2.left;

        // 🔥 Pozycja pocisku — obok gracza w kierunku strzału
        Vector3 spawnPosition = player.transform.position + (Vector3)(shootDirection * 1.2f);

        // 🔥 Tworzenie pocisku
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // 🔥 Przekazujemy kierunek do pocisku
        bullet.GetComponent<Bullet>().Initialize(shootDirection);


        // 🔥 Cooldown na strzał
        cooldownManager.StartCooldown("Shoot", shootCooldown, shootIcon);
        StartCoroutine(ResetAttackCooldown(nameof(canShoot), shootCooldown));
    }

    private void PerformStrongShoot()
    {
        if (player == null) return;
        if (isPerformingSpecialAttack || !canStrongShoot) return;
        canStrongShoot = false;

        Debug.Log("💥 Gracz wykonuje MOCNY STRZAŁ!");

        // 🔥 Kierunek strzału na podstawie `lastMoveDirection`
        Vector2 shootDirection = lastMoveDirection.x >= 0 ? Vector2.right : Vector2.left;
        Vector3 spawnPosition = player.transform.position + (Vector3)(shootDirection * 1.2f);

        // 🔥 Tworzenie mocnego pocisku
        GameObject bullet = Instantiate(strongBulletPrefab, spawnPosition, Quaternion.identity);
        bullet.GetComponent<StrongBullet>().Initialize(shootDirection);

        // 🔥 Gracz lekko odrzucony w przeciwną stronę
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockback = new Vector2(-shootDirection.x * 50f, 3f); // Lekkie odepchnięcie
            playerRb.velocity = Vector2.zero; // Reset prędkości
            playerRb.AddForce(knockback, ForceMode2D.Impulse);
        }

        // 🔥 Cooldown na mocny strzał
        cooldownManager.StartCooldown("StrongShoot", strongShootCooldown, strongShootIcon);
        StartCoroutine(ResetAttackCooldown(nameof(canStrongShoot), strongShootCooldown));
    }

    IEnumerator WeakAttackWithDash(Vector2 direction)
    {
        if (isPerformingSpecialAttack || !canWeakAttack) yield break;
        
        isPerformingSpecialAttack = true;
        canWeakAttack = false;
        StartCoroutine(Dash(direction));
        yield return new WaitForSeconds(dashDuration);
        WeakAttack(direction);
        isPerformingSpecialAttack = false;
    }

    IEnumerator Dash(Vector2 direction)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            rb.velocity = direction * dashSpeed;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(dashCooldown);
    }
}

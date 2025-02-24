using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 15f;
    private Rigidbody2D rb;
    private bool isGrounded;
    public Transform spawnPoint;

    public Transform attackPoint; // Punkt ataku (przeciągnij AttackPoint w Inspectorze)
    public float attackRange = 1f; // Zasięg ataku
    public float attackForce = 5f; // Siła odrzutu wroga
    public LayerMask enemyLayers; // Warstwa przeciwników

    private Vector2 lastMoveDirection = Vector2.right; // Domyślnie atakujemy w prawo

    private HealthBar healthBar;
    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false; // Czy gracz ma niewrażliwość?
    public float invincibilityDuration = 3f; // Czas niewrażliwości
    public float knockbackForce = 5f; // Siła odrzutu po trafieniu
    private SpriteRenderer spriteRenderer;

    public float regenTime = 3f; // hp regen

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBar = GetComponentInChildren<HealthBar>();
        
        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth);
        }

        InvokeRepeating(nameof(RegenerateHealth), regenTime, regenTime);
    }

    void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            Debug.Log("Gracz odzyskał 1 HP! Aktualne HP: " + currentHealth);

            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth); // Aktualizacja paska HP
            }
        }
    }

    void Update()
    {
        // Ruch w lewo/prawo
        float move = 0;

        if (Input.GetKey(KeyCode.LeftArrow)) // Natychmiastowy ruch w lewo
        {
            move = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow)) // Natychmiastowy ruch w prawo
        {
            move = 1;
        }
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        if (move != 0) // Jeśli gracz się porusza, zapamiętaj kierunek
        {
            lastMoveDirection = new Vector2(move, 0).normalized;
        }

        // Skok
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        //Atak
        if (Input.GetKeyDown(KeyCode.W)) // Atak po naciśnięciu W
        {
            Attack();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }

    public void Respawn()
    {
        transform.position = spawnPoint.position; // Przenosi gracza na punkt startowy
        rb.velocity = Vector2.zero; // Zeruje prędkość, żeby gracz nie "ślizgał się"
    }

    public void FixedUpdate()
    {
        if (isGrounded) // Jeśli gracz dotknął ziemi, powoli wracaj do pionu
        {
            float smoothRotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, 0, Time.fixedDeltaTime * 8f);
            transform.rotation = Quaternion.Euler(0, 0, smoothRotation);
        }
    }

    void Attack()
    {
        Debug.Log("Gracz atakuje!");

        // Przesunięcie AttackPoint w stronę, w którą gracz ostatnio się poruszał
        attackPoint.position = transform.position + (Vector3)(lastMoveDirection * attackRange);

        // Tymczasowa animacja - kwadrat pojawia się na chwilę
        GameObject attackEffect = GameObject.CreatePrimitive(PrimitiveType.Quad);
        attackEffect.transform.position = attackPoint.position;
        attackEffect.transform.position += new Vector3(0, 0, -1);
        attackEffect.transform.localScale = new Vector3(attackRange, attackRange, 1);
        attackEffect.GetComponent<Renderer>().sortingLayerName = "Foreground";
        attackEffect.GetComponent<Renderer>().sortingOrder = 5;
        attackEffect.GetComponent<Renderer>().material.color = Color.red;
        Destroy(attackEffect, 0.1f); // Po 0.2 sekundy znika

        // Wykrywamy wrogów w zasięgu ataku
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Trafiono: " + enemy.name);

            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null && enemyRb.bodyType == RigidbodyType2D.Dynamic) // Tylko jeśli jest Dynamic
            {
            // 🔥 Obliczamy kierunek odpychania (wróg -> gracz)
            Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;

            // 🔥 Ustawiamy siłę odrzutu w przeciwnym kierunku od gracza
            Vector2 knockback = knockbackDirection * attackForce;
            knockback.y = Mathf.Abs(knockback.y) + 2f; // Mały podskok dla lepszego efektu

            enemyRb.velocity = Vector2.zero; // Zerujemy prędkość przed odrzutem
            enemyRb.AddForce(knockback, ForceMode2D.Impulse);

            Debug.Log($"Wróg odrzucony kierunek: {knockback}");
            }
            else
            {
                Debug.Log("Nie można odepchnąć wroga, brak Rigidbody2D lub nie jest Dynamic!");
            }

            EnemyChaseAI enemyChase = enemy.GetComponent<EnemyChaseAI>();
            EnemyAI enemyPatrol = enemy.GetComponent<EnemyAI>();

            if (enemyChase != null) {
                enemyChase.TakeDamage(1);
            } 
            else if (enemyPatrol != null) 
            {
                enemyPatrol.TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int damage, Transform enemy)
    {
        if (isInvincible) return; // Jeśli jest niewrażliwy, ignorujemy obrażenia
        currentHealth -= damage;
        Debug.Log("Gracz otrzymał " + damage + " obrażeń! HP: " + currentHealth);
        
        // if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Gracz zginął!");
            Respawn();
            // Dodaj system respawn
        }

        // 🔥 Odrzut gracza w stronę przeciwną do wroga
        Vector2 knockbackDirection = (transform.position - enemy.position).normalized;
        rb.velocity = Vector2.zero; // Resetujemy prędkość, aby odrzut był widoczny
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // 🔥 Uruchamiamy niewrażliwość
        StartCoroutine(InvincibilityFrames());
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        float elapsedTime = 0f;

        while (elapsedTime < invincibilityDuration)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f); // Przezroczystość 50%
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.red; // Wraca do normalnego koloru
            yield return new WaitForSeconds(0.2f);
            elapsedTime += 0.4f;
        }

        isInvincible = false;
    }

}

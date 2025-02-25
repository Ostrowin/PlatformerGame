using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 15f;
    private Rigidbody2D rb;
    private bool isGrounded;
    public Transform spawnPoint;

    public Transform attackPoint; // Punkt ataku (przeciągnij AttackPoint w Inspectorze)
    public float attackRange = 1f; // Zasięg ataku
    public float attackForce = 5f; // Siła odrzutu wroga
    public LayerMask enemyLayers; // Warstwa przeciwników

    private HealthBar healthBar;
    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false; // Czy gracz ma niewrażliwość?
    public float invincibilityDuration = 1f; // Czas niewrażliwości
    public float knockbackForce = 5f; // Siła odrzutu po trafieniu
    private SpriteRenderer spriteRenderer;

    public float regenTime = 3f; // hp regen
    
    // 🔥 Wall Jump & Wall Stick
    public Transform wallCheckLeft, wallCheckRight; // Dwa punkty sprawdzające ściany    public float wallCheckRadius = 0.2f;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer; // Warstwa, na której wykrywamy ściany
    private bool isTouchingWall;
    private bool isWallSliding;
    public float wallSlideSpeed = 1f; // Prędkość zsuwania się po ścianie
    public float wallJumpForce = 10f;
    public float wallJumpXForce = 15f;

    public bool isDashing = false;

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
        if (isDashing) return; // Jeśli trwa Dash, ignorujemy standardowy ruch

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

        // 🔥 Wall Stick Mechanic
        isTouchingWall = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer) || 
                 Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);

        if (isTouchingWall && Input.GetAxisRaw("Horizontal") != 0 && rb.velocity.y < 0)
        {
            isWallSliding = true;
            Debug.Log("isWallSliding: " + isWallSliding);
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }

        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");
            if (isWallSliding)
            {
                Debug.Log("isWallSliding");
                WallJump(); // 🔥 Wykonaj Wall Jump, jeśli gracz jest na ścianie
            }
            else if (isGrounded)
            {
                Debug.Log("isGrounded");
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }

        // 🔥 Jeśli gracz nie jest na ziemi i nie ślizga się po ścianie, powoli wracamy do pionu
        if (!isGrounded && !isWallSliding)
        {
            float smoothRotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, 0, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Euler(0, 0, smoothRotation);
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

   void WallJump()
    {
        Debug.Log("WallJump");
        
        bool touchingLeftWall = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer);
        bool touchingRightWall = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);

        if (touchingLeftWall || touchingRightWall)
        {
            float jumpDirection = touchingLeftWall ? 1 : -1;

            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(jumpDirection * wallJumpXForce, wallJumpForce), ForceMode2D.Impulse);
            rb.velocity = new Vector2(jumpDirection * wallJumpXForce * 3f, wallJumpForce);

            Debug.Log($"Wall Jump! Kierunek: {jumpDirection}");
        }
    }
}

using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // ======================== Ruch ========================
    private Rigidbody2D rb;
    private SprintHandler sprintHandler;
    public bool isDashing = false;
    
    // ======================== Skakanie & Ślizganie po Ścianie ========================
    public float jumpForce = 15f;
    public Transform wallCheckLeft, wallCheckRight;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;
    private bool isTouchingWall;
    private bool isWallSliding;
    public float wallSlideSpeed = 1f;
    public float wallJumpForce = 10f;
    public float wallJumpXForce = 15f;
    
    // ======================== Atak ========================
    public Transform attackPoint;
    public float attackRange = 1f;
    public float attackForce = 5f;
    public LayerMask enemyLayers;
    
    // ======================== Zdrowie ========================
    private HealthBar healthBar;
    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false;
    public float invincibilityDuration = 1f;
    public float knockbackForce = 5f;
    private SpriteRenderer spriteRenderer;
    public float regenTime = 3f;
    
    // ======================== Inne ========================
    public Transform spawnPoint;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprintHandler = GetComponent<SprintHandler>();
        if (sprintHandler == null)
        {
            Debug.LogWarning("SprintHandler nie został znaleziony! Automatycznie dodaję komponent.");
            sprintHandler = gameObject.AddComponent<SprintHandler>();
        }

        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBar = GetComponentInChildren<HealthBar>();

        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth);
        }

        InvokeRepeating(nameof(RegenerateHealth), regenTime, regenTime);
    }

    void Update()
    {
        if (isDashing) return;

        // Ruch
        float move = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) move = -1;
        else if (Input.GetKey(KeyCode.RightArrow)) move = 1;
        
        rb.velocity = new Vector2(move * sprintHandler.GetCurrentSpeed(), rb.velocity.y);

        // Ślizganie po ścianie
        isTouchingWall = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer) ||
                         Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);
        isWallSliding = isTouchingWall && Input.GetAxisRaw("Horizontal") != 0 && rb.velocity.y < 0;
        if (isWallSliding) rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);

        // Skok
        if (Input.GetButtonDown("Jump"))
        {
            if (isWallSliding) WallJump();
            else if (isGrounded) rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
        // 🔥 Jeśli gracz nie jest na ziemi i nie ślizga się po ścianie, powoli wracamy do pionu
        if (!isGrounded && !isWallSliding)
        {
            float smoothRotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, 0, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Euler(0, 0, smoothRotation);
        }
        FixedUpdate();
    }

    void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            if (healthBar != null) healthBar.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(int damage, Transform enemy)
    {
        if (isInvincible) return;
        currentHealth -= damage;
        Debug.Log("Gracz otrzymał " + damage + " obrażeń! HP: " + currentHealth);
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Gracz zginął!");
            Respawn();
        }

        Vector2 knockbackDirection = (transform.position - enemy.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(InvincibilityFrames());
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        float elapsedTime = 0f;

        while (elapsedTime < invincibilityDuration)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            elapsedTime += 0.4f;
        }

        isInvincible = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) isGrounded = false;
    }

    public void Respawn()
    {
        transform.position = spawnPoint.position;
        rb.velocity = Vector2.zero;
    }

    void WallJump()
    {
        bool touchingLeftWall = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer);
        bool touchingRightWall = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);

        if (touchingLeftWall || touchingRightWall)
        {
            float jumpDirection = touchingLeftWall ? 1 : -1;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(jumpDirection * wallJumpXForce, wallJumpForce), ForceMode2D.Impulse);
            rb.velocity = new Vector2(jumpDirection * wallJumpXForce * 3f, wallJumpForce);
        }
    }

    public void FixedUpdate()
    {
        if (isGrounded) // Jeśli gracz dotknął ziemi, powoli wracaj do pionu
        {
            float smoothRotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, 0, Time.fixedDeltaTime * 8f);
            transform.rotation = Quaternion.Euler(0, 0, smoothRotation);
        }
    }
}

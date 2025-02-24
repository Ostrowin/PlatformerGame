using UnityEngine;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Ruch w lewo/prawo
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

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

    // Tymczasowa animacja - kwadrat pojawia się na chwilę
    GameObject attackEffect = GameObject.CreatePrimitive(PrimitiveType.Quad);
    attackEffect.transform.position = attackPoint.position;
    attackEffect.transform.localScale = new Vector3(attackRange, attackRange, 1);
    Destroy(attackEffect, 0.2f); // Po 0.2 sekundy znika

    // Wykrywamy wrogów w zasięgu ataku
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

    foreach (Collider2D enemy in hitEnemies)
    {
        Debug.Log("Trafiono: " + enemy.name);

        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null && enemyRb.bodyType == RigidbodyType2D.Dynamic) // Tylko jeśli jest Dynamic
        {
            Vector2 knockback = new Vector2(transform.localScale.x * attackForce, 3f); // Poprawiona siła odrzutu
            enemyRb.velocity = Vector2.zero; // Zerujemy prędkość, by efekt był widoczny
            enemyRb.AddForce(knockback, ForceMode2D.Impulse);
            Debug.Log("Wróg odrzucony!");
        }
        else
        {
            Debug.Log("Nie można odepchnąć wroga, brak Rigidbody2D lub nie jest Dynamic!");
        }
    }
}

}

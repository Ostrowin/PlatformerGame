using UnityEngine;
using System.Collections;

public class EnemyChaseAI : MonoBehaviour
{
    public Transform player; // Gracz
    public float speed = 3f; // Prędkość gonienia
    public float chaseRange = 5f; // Zasięg wykrywania gracza
    public float stopChaseRange = 7f; // Odległość, po której wróg rezygnuje
    public Transform patrolPointA; // Pierwszy punkt patrolu
    public Transform patrolPointB; // Drugi punkt patrolu

    private Transform target;
    private bool isChasing = false;

    
    public int maxHealth = 5;
    private int currentHealth;
    private HealthBar healthBar; 

    public float attackRange = 1.2f; // Zasięg ataku
    public float attackCooldown = 2f; // Czas między atakami
    public int attackDamage = 1; // Obrażenia zadawane przez wroga
    public float knockbackForce = 5f; // Siła odrzutu gracza
    private bool canAttack = true; // Czy wróg może atakować
    public float regenTime = 5f; // hp regen

    void Start()
    {
        target = patrolPointA; // Wróg zaczyna patrolować
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(currentHealth);
            Debug.Log("Zainicjalizowano pasek HP dla: " + name + " z HP: " + currentHealth);
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
            Debug.Log(name + " odzyskał 1 HP! Aktualne HP: " + currentHealth);

            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth);
            }
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < chaseRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > stopChaseRange)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer(); // Jeśli gracz jest dalej, wróg goni
            }
            else if (canAttack)
            {
                StartCoroutine(AttackPlayer()); // Jeśli blisko, atakuje
            }
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == patrolPointA) ? patrolPointB : patrolPointA;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Odejmujemy HP
        Debug.Log(name + " otrzymał " + damage + " obrażeń! HP: " + currentHealth);

        if (healthBar != null)
        {
            Debug.Log("Aktualizacja paska HP dla: " + name);
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            Debug.LogWarning("Brak komponentu HealthBar dla: " + name);
        }
        // Jeśli wróg ma SpriteRenderer, zapisujemy jego oryginalny kolor i migamy na czerwono
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            StartCoroutine(DamageEffect(sprite));
        }

        // Jeśli HP spadnie do 0, wróg umiera
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageEffect(SpriteRenderer sprite)
    {
        Color originalColor = sprite.color; // Zapamiętujemy kolor
        sprite.color = Color.red; // Wróg zmienia kolor na czerwony
        yield return new WaitForSeconds(0.2f);
        sprite.color = originalColor; // Wracamy do oryginalnego koloru
    }

    void Die()
    {
        Debug.Log(name + " zginął!");
        Destroy(gameObject); // Usuwamy obiekt
    }

    IEnumerator AttackPlayer()
    {
        canAttack = false; // Blokujemy atak na czas cooldownu
        Debug.Log("Wróg atakuje gracza!");

        // 🔥 Tworzymy prostokąt ataku
        GameObject attackEffect = GameObject.CreatePrimitive(PrimitiveType.Quad);
        attackEffect.transform.position = transform.position + new Vector3(transform.localScale.x * 1f, 0, 0); // Umieszczamy przed wrogiem
        attackEffect.transform.localScale = new Vector3(attackRange, attackRange, 1); // Rozmiar prostokąta
        attackEffect.GetComponent<Renderer>().material.color = Color.red; // Kolor czerwony
        attackEffect.GetComponent<Renderer>().sortingLayerName = "Foreground";
        attackEffect.GetComponent<Renderer>().sortingOrder = 5;
        attackEffect.transform.position += new Vector3(0, 0, -1);
        Destroy(attackEffect, 0.2f); // Usuwamy po 0.2 sekundy
        if (player != null)
        {
            PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(attackDamage, transform);
            }
        }

        yield return new WaitForSeconds(attackCooldown); // Czekamy na cooldown ataku
        canAttack = true;
    }
}

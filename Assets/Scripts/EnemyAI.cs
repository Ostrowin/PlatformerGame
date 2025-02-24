using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float speed = 4f; // Prędkość poruszania się
    public Transform pointA; // Pierwszy punkt patrolu
    public Transform pointB; // Drugi punkt patrolu
    private Transform target;

    public int maxHealth = 5;
    private int currentHealth;
    private HealthBar healthBar; 
    public float regenTime = 5f; // hp regen

    void Start()
    {
        target = pointB; // Zaczynamy od ruchu w stronę pointB
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(currentHealth);
            Debug.Log("Zainicjalizowano pasek HP dla: " + name + " z HP: " + currentHealth);
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
                healthBar.SetHealth(currentHealth); // Aktualizacja paska HP
            }
        }
    }

    void Update()
    {
        // isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        
        // Zmiana kierunku, gdy wróg dotrze do celu
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     Debug.Log("Kolizja z: " + collision.gameObject.name); // Wyświetli w konsoli nazwę obiektu, który dotknął wroga


    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         Debug.Log("Wróg zadaje obrażenia graczowi!");
    //         collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(1, transform);
    //     }
    // }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Kolizja TRIGGER z: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
                Debug.Log("Wróg zadaje obrażenia graczowi!");
            collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(1, transform);
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

}

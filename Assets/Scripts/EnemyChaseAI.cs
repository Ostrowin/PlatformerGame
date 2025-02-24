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

    public int health = 3; // HP wroga
    private HealthBar healthBar; 

    void Start()
    {
        target = patrolPointA; // Wróg zaczyna patrolować
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(health);
            Debug.Log("Zainicjalizowano pasek HP dla: " + name + " z HP: " + health);
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
            ChasePlayer();
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
        health -= damage; // Odejmujemy HP
        Debug.Log(name + " otrzymał " + damage + " obrażeń! HP: " + health);

        if (healthBar != null)
        {
            Debug.Log("Aktualizacja paska HP dla: " + name);
            healthBar.SetHealth(health);
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
        if (health <= 0)
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

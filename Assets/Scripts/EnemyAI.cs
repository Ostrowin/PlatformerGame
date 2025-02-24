using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 4f; // Prędkość poruszania się
    public Transform pointA; // Pierwszy punkt patrolu
    public Transform pointB; // Drugi punkt patrolu
    private Transform target;

    // public Transform groundCheck; // Punkt sprawdzający ziemię
    // public LayerMask groundLayer; // Warstwa podłoża
    // private bool isGrounded;

    void Start()
    {
        target = pointB; // Zaczynamy od ruchu w stronę pointB
    }

    void Update()
    {
        // isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // if (isGrounded)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, 0)); // Blokujemy spadanie przez ziemię
        // }
        
        // Zmiana kierunku, gdy wróg dotrze do celu
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Kolizja z: " + collision.gameObject.name); // Wyświetli w konsoli nazwę obiektu, który dotknął wroga

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Gracz dotknął przeciwnika! Resetowanie pozycji...");
            collision.gameObject.GetComponent<PlayerMovement>().Respawn();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Kolizja TRIGGER z: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Gracz dotknął przeciwnika! Resetowanie pozycji...");
            collision.GetComponent<PlayerMovement>().Respawn();
        }
    }

}

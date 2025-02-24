using UnityEngine;

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

//     public Transform groundCheck; // Punkt sprawdzający ziemię
//     public LayerMask groundLayer; // Warstwa podłoża
//   private bool isGrounded;


    void Start()
    {
        target = patrolPointA; // Wróg zaczyna patrolować
    }

    void Update()
    {
        //   isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
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
}

using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 4f;
    private Transform target;
    private EnemyChase chaseComponent;

    void Start()
    {
        target = pointB;
        chaseComponent = GetComponent<EnemyChase>();
    }

    void Update()
    {
        if (chaseComponent != null && chaseComponent.IsChasing) return;

        Patrol();
    }

    private void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }
}

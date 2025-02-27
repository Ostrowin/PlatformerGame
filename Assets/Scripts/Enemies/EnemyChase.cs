using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float chaseRange = 5f;
    public float stopChaseRange = 7f;
    private bool isChasing = false;

    public bool IsChasing => isChasing;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < chaseRange)
        {
            isChasing = true;
            Move();
        }
        else if (distanceToPlayer > stopChaseRange)
        {
            isChasing = false;
        }
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }
}

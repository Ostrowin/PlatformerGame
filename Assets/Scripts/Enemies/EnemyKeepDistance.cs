using UnityEngine;

public class EnemyKeepDistance : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float minDistance = 5f; // Minimalna odległość od gracza
    public float maxDistance = 10f; // Maksymalna odległość do strzelania

    private EnemyEdgeDetection edgeDetection; // 🔥 Pobieramy komponent do wykrywania krawędzi

    void Start()
    {
        edgeDetection = GetComponent<EnemyEdgeDetection>(); // Pobieramy EdgeDetection
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < minDistance)
        {
            if (edgeDetection == null || (!edgeDetection.IsNearLeftEdge && !edgeDetection.IsNearRightEdge)) 
            {
                MoveAwayFromPlayer();
            }
        }
        else if (distance > maxDistance)
        {
            if (edgeDetection == null || (!edgeDetection.IsNearLeftEdge && !edgeDetection.IsNearRightEdge)) 
            {
                MoveTowardsPlayer();
            }
        }
    }

    private void MoveAwayFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }
}

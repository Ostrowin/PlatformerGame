using UnityEngine;

public class EnemyEdgeDetection : MonoBehaviour
{
    [Header("Edge Detection Settings")]
    public Transform leftEdgeCheck;  // 🔥 Punkt sprawdzający krawędź po lewej
    public Transform rightEdgeCheck; // 🔥 Punkt sprawdzający krawędź po prawej
    public LayerMask groundLayer;    // 🔥 Warstwa podłoża
    public float checkDistance = 2f; // 🔥 Jak daleko sprawdzamy przed sobą

    private bool isNearLeftEdge = false;
    private bool isNearRightEdge = false;

    public bool IsNearLeftEdge => isNearLeftEdge;  // Getter dla lewej krawędzi
    public bool IsNearRightEdge => isNearRightEdge; // Getter dla prawej krawędzi

    void Update()
    {
        CheckForEdge();
    }

    private void CheckForEdge()
    {
        if (leftEdgeCheck != null)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(leftEdgeCheck.position, Vector2.down, checkDistance, groundLayer);
            isNearLeftEdge = hitLeft.collider == null;
            Debug.DrawRay(leftEdgeCheck.position, Vector2.down * checkDistance, isNearLeftEdge ? Color.red : Color.green);
        }

        if (rightEdgeCheck != null)
        {
            RaycastHit2D hitRight = Physics2D.Raycast(rightEdgeCheck.position, Vector2.down, checkDistance, groundLayer);
            isNearRightEdge = hitRight.collider == null;
            Debug.DrawRay(rightEdgeCheck.position, Vector2.down * checkDistance, isNearRightEdge ? Color.red : Color.green);
        }
    }
}

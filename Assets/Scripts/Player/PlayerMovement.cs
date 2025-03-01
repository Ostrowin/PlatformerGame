using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SprintHandler sprintHandler;
    public bool isDashing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprintHandler = GetComponent<SprintHandler>() ?? gameObject.AddComponent<SprintHandler>();
    }

    void Update()
    {
        if (isDashing) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        float move = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) move = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) move = 1f;

        rb.velocity = new Vector2(move * sprintHandler.GetCurrentSpeed(), rb.velocity.y);
    }
}

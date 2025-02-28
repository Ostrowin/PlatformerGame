using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumpForce = 15f;
    public Transform wallCheckLeft, wallCheckRight;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;
    private bool isWallSliding;
    public float wallSlideSpeed = 1f;
    public float wallJumpForce = 10f;
    public float wallJumpXForce = 15f;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleWallSliding();
        HandleJumping();
    }

    private void HandleJumping()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isWallSliding)
                WallJump();
            else if (isGrounded)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void HandleWallSliding()
    {
        bool isTouchingWall = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer) ||
                              Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);

        isWallSliding = isTouchingWall && Input.GetAxisRaw("Horizontal") != 0 && rb.velocity.y < 0;

        if (isWallSliding)
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
    }

    void WallJump()
    {
        float jumpDirection = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer) ? 1 : -1;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(jumpDirection * wallJumpXForce, wallJumpForce), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = false;
    }
}

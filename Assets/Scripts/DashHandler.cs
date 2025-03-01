using UnityEngine;
using System.Collections;

public class DashHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    public float dashForce = 30f;
    public float dashCooldown = 1.5f;
    private bool canDash = true;
    public ParticleSystem dashEffect;
    private CooldownSystem cooldownSystem;
    private CooldownUI cooldownUI;
    public Sprite dashIcon; // Ikona dla Dasha

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cooldownSystem = FindObjectOfType<CooldownSystem>();
        cooldownUI = FindObjectOfType<CooldownUI>();

        FindObjectOfType<KeyCombinationManager>().RegisterCombination(
            new KeyCode[] { KeyCode.A, KeyCode.LeftArrow, KeyCode.D }, 
            () => Dash(-1)
        );

        FindObjectOfType<KeyCombinationManager>().RegisterCombination(
            new KeyCode[] { KeyCode.A, KeyCode.RightArrow, KeyCode.D }, 
            () => Dash(1)
        );
    }

    void Dash(int direction)
    {
        if (!canDash) return;

        canDash = false;
        cooldownSystem.StartCooldown("Dash", dashCooldown);
        cooldownUI.AddCooldown("Dash", dashCooldown, dashIcon);

        PlayerMovement player = GetComponent<PlayerMovement>();
        player.isDashing = true;

        rb.gravityScale = 0; // 🔥 Wyłączamy grawitację na czas Dasha
        rb.velocity = new Vector2(direction * dashForce, 0); // Dash poziomy
        
        // 🔥 Efekt cząsteczkowy
        if (dashEffect != null) dashEffect.Play();

        StartCoroutine(EndDash());
    }

    IEnumerator EndDash()
    {
        yield return new WaitForSeconds(0.2f); // Czas trwania Dasha

        rb.gravityScale = 3; // 🔥 Przywracamy grawitację
        GetComponent<PlayerMovement>().isDashing = false;
        rb.velocity = Vector2.zero; // Zatrzymanie gracza po Dashu

        // 🔥 Wyłącz efekt cząsteczkowy
        if (dashEffect != null) dashEffect.Stop();

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}

using UnityEngine;

public class SprintHandler : MonoBehaviour
{
    public float normalSpeed = 5f;
    public float sprintSpeed = 8f;
    private float currentSpeed;
    private Rigidbody2D rb;
    private CooldownManager cooldownManager;
    public Sprite staminaIcon; // Ikona staminy

    private bool isSprinting;
    private bool staminaCooldownActive = false;

    // ======================== Stamina ========================
    public float maxStamina = 100f;
    private float currentStamina;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 10f;
    public bool canSprint = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cooldownManager = FindObjectOfType<CooldownManager>();
        currentSpeed = normalSpeed;
        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleSprint();
        RegenerateStamina();
        UpdateCooldownUI();
    }

    private void HandleSprint()
    {
        bool movePressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        bool sprintPressed = Input.GetKey(KeyCode.D);

        if (movePressed && sprintPressed && canSprint)
        {
            if (currentStamina > 0)
            {
                isSprinting = true;
                currentSpeed = sprintSpeed;
                currentStamina -= staminaDrainRate * Time.deltaTime;
                
                if (!staminaCooldownActive)
                {
                    cooldownManager.StartCooldown("Stamina", staminaIcon);
                    staminaCooldownActive = true;
                }
            }
            else
            {
                isSprinting = false;
                currentSpeed = normalSpeed;
                canSprint = false;
            }
        }
        else
        {
            isSprinting = false;
            currentSpeed = normalSpeed;
        }
    }

    private void RegenerateStamina()
    {
        if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina >= maxStamina)
            {
                canSprint = true;
                cooldownManager.RemoveCooldown("Stamina");
                staminaCooldownActive = false;
            }
        }
    }

    private void UpdateCooldownUI()
    {
        if (staminaCooldownActive)
        {
            float percentage = currentStamina / maxStamina;
            cooldownManager.UpdateCooldown("Stamina", percentage);
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}

using UnityEngine;

public class SprintHandler : MonoBehaviour
{
    public float normalSpeed = 5f;
    public float sprintSpeed = 8f;
    private float currentSpeed;
    private Rigidbody2D rb;
    private CooldownSystem cooldownSystem;
    private CooldownUI cooldownUI;
    public Sprite staminaIcon;

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
        cooldownSystem = FindObjectOfType<CooldownSystem>();
        cooldownUI = FindObjectOfType<CooldownUI>();
        currentSpeed = normalSpeed;
        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleSprint();
        RegenerateStamina();
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
                    cooldownSystem.StartCooldown("Stamina");
                    cooldownUI.AddCooldown("Stamina", staminaIcon);
                    staminaCooldownActive = true;
                }
                UpdateCooldown();
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

            UpdateCooldown();

            if (currentStamina >= maxStamina)
            {
                canSprint = true;
                staminaCooldownActive = false;
                cooldownSystem.RemoveCooldown("Stamina");
            }
        }
    }

    private void UpdateCooldown()
    {
        float percentage = currentStamina / maxStamina;
        cooldownSystem.SetCooldownPercentage("Stamina", percentage);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}

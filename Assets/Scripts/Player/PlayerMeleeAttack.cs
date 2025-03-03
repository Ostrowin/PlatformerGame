using UnityEngine;
using System.Collections;

public class PlayerMeleeAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float strongAttackRange = 1f;
    public float weakAttackRange = 1.5f;
    public float attackForce = 7f;
    public LayerMask enemyLayers;

    public float basicAttackCooldown = 0.5f;
    public float strongAttackCooldown = 2f;
    public float weakAttackCooldown = 1f;
    public Sprite strongAttackIcon;
    public Sprite weakAttackIcon;

    private CooldownSystem cooldownSystem;
    private CooldownUI cooldownUI;

    private KeyCombinationManager keyManager;
    private PlayerDirection playerDirection;
    private PlayerDash playerDash; 

    public GameObject slashEffectPrefab;

    private void Start()
    {
        cooldownSystem = FindObjectOfType<CooldownSystem>();
        cooldownUI = FindObjectOfType<CooldownUI>();

        keyManager = FindObjectOfType<KeyCombinationManager>();
        playerDirection = GetComponent<PlayerDirection>();
        playerDash = GetComponent<PlayerDash>();

        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.LeftArrow, KeyCode.E }, () => PerformStrongAttack(Vector2.left));
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.RightArrow, KeyCode.E }, () => PerformStrongAttack(Vector2.right));
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.UpArrow, KeyCode.E }, () => PerformStrongAttack(Vector2.up));

        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.LeftArrow, KeyCode.E }, () => PerformWeakAttack(Vector2.left));
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.RightArrow, KeyCode.E }, () => PerformWeakAttack(Vector2.right));
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.UpArrow, KeyCode.E }, () => PerformWeakAttack(Vector2.up));

        keyManager.RegisterCombination(new KeyCode[] { KeyCode.E }, () => PerformBasicAttack());
    }

    public void PerformBasicAttack()
    {
        Debug.Log("PerformBasicAttack");
        if (cooldownSystem.IsOnCooldown("Basic Attack")) return;
        
        Vector2 attackDirection = playerDirection.lastMoveDirection;
        PerformAttack(attackDirection, 1f, attackForce / 2, 2);
        cooldownSystem.StartCooldown("Basic Attack", basicAttackCooldown);
    }

    public void PerformStrongAttack(Vector2 direction)
    {
        if (cooldownSystem.IsOnCooldown("Strong Attack")) return;

        cooldownSystem.StartCooldown("Strong Attack", strongAttackCooldown);
        cooldownUI.AddCooldown("Strong Attack", strongAttackCooldown, strongAttackIcon);
        PerformAttack(direction, strongAttackRange, attackForce, 3);
    }

    public void PerformWeakAttack(Vector2 direction)
    {
        if (cooldownSystem.IsOnCooldown("Weak Attack")) return;

        Debug.Log($"⚡ Wykonano słaby atak w kierunku {direction}");

        playerDash.PerformDash(direction);
        
        PerformAttack(direction, 1.5f, attackForce / 2, 1);
        cooldownSystem.StartCooldown("Weak Attack", weakAttackCooldown);
    }

    private void PerformAttack(Vector2 direction, float range, float force, int damage)
    {
        Debug.Log("PerformAttack");
        Vector3 attackPosition = transform.position + (Vector3)(direction * range);
        ShowSlashEffect(attackPosition, playerDirection.lastMoveDirection.x > 0);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, range, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
            }

            if (enemy.TryGetComponent(out Rigidbody2D enemyRb))
            {
                Vector2 knockback = direction.normalized * force;
                enemyRb.velocity = Vector2.zero;
                enemyRb.AddForce(knockback, ForceMode2D.Impulse);
            }
        }
    }

    private void ShowSlashEffect(Vector2 position, bool facingRight)
    {
        GameObject slash = Instantiate(slashEffectPrefab, position, Quaternion.identity);
        
        // Jeśli gracz patrzy w lewo, odwróć efekt
        if (!facingRight)
            slash.transform.localScale = new Vector3(-1, 1, 1);

        // Uruchom Particle System
        ParticleSystem ps = slash.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        Destroy(slash, 0.3f); // Usuń efekt po czasie
    }

}

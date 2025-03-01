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

    private void Start()
    {
        cooldownSystem = FindObjectOfType<CooldownSystem>();
        cooldownUI = FindObjectOfType<CooldownUI>();
    }

    public void PerformBasicAttack(Vector2 direction)
    {
        if (cooldownSystem.IsOnCooldown("Basic Attack")) return;

        PerformAttack(direction, 1f, attackForce / 2, 2);
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

        cooldownSystem.StartCooldown("Weak Attack", weakAttackCooldown);
        cooldownUI.AddCooldown("Weak Attack", weakAttackCooldown, weakAttackIcon);
        PerformAttack(direction, weakAttackRange, attackForce / 2, 1);
    }

    private void PerformAttack(Vector2 direction, float range, float force, int damage)
    {
        Vector3 attackPosition = transform.position + (Vector3)(direction * range);
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
}

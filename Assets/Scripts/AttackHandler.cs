using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public Transform attackPoint;
    public float strongAttackRange = 1f;
    public float weakAttackRange = 1.5f;
    public float attackForce = 7f;
    public LayerMask enemyLayers;

    void Start()
    {
        var keyManager = FindObjectOfType<KeyCombinationManager>();

        // 🔥 Silny atak w lewo i prawo
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.LeftArrow, KeyCode.E }, () => StrongAttack(Vector2.left));
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.RightArrow, KeyCode.E }, () => StrongAttack(Vector2.right));

        // 🔥 Silny atak w górę
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.W, KeyCode.UpArrow, KeyCode.E }, () => StrongAttack(Vector2.up));

        // 🔥 Słabszy atak o dalszym zasięgu w bok
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.LeftArrow, KeyCode.E }, () => WeakAttack(Vector2.left));
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.RightArrow, KeyCode.E }, () => WeakAttack(Vector2.right));

        // 🔥 Słabszy atak o dalszym zasięgu w górę
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.Q, KeyCode.UpArrow, KeyCode.E }, () => WeakAttack(Vector2.up));
    }

    void StrongAttack(Vector2 direction)
    {
        Debug.Log("Silny atak w kierunku: " + direction);
        PerformAttack(direction, strongAttackRange, attackForce, Color.yellow); // 🔥 Złoty kolor
    }

    void WeakAttack(Vector2 direction)
    {
        Debug.Log("Słabszy atak w kierunku: " + direction);
        PerformAttack(direction, weakAttackRange, attackForce / 2, new Color(0.5f, 0f, 0.5f)); // 🔥 Fioletowy kolor
    }

    void PerformAttack(Vector2 direction, float range, float force, Color attackColor)
    {
        Vector3 attackPosition = transform.position + (Vector3)(direction * range);

        // 🔥 Tymczasowa wizualizacja ataku
        GameObject attackEffect = GameObject.CreatePrimitive(PrimitiveType.Quad);
        attackEffect.transform.position = attackPosition;
        attackEffect.transform.localScale = new Vector3(range, range, 1);
        attackEffect.GetComponent<Renderer>().sortingLayerName = "Foreground";
        attackEffect.GetComponent<Renderer>().sortingOrder = 5;
        attackEffect.GetComponent<Renderer>().material.color = attackColor; // 🔥 Ustawienie koloru
        Destroy(attackEffect, 0.1f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, range, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Trafiono: " + enemy.name);

            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null && enemyRb.bodyType == RigidbodyType2D.Dynamic)
            {
                Vector2 knockbackDirection = direction.normalized;
                Vector2 knockback = knockbackDirection * force;
                knockback.y = Mathf.Abs(knockback.y) + 2f;

                enemyRb.velocity = Vector2.zero;
                enemyRb.AddForce(knockback, ForceMode2D.Impulse);
            }

            EnemyChaseAI enemyChase = enemy.GetComponent<EnemyChaseAI>();
            EnemyAI enemyPatrol = enemy.GetComponent<EnemyAI>();

            if (enemyChase != null) enemyChase.TakeDamage(1);
            else if (enemyPatrol != null) enemyPatrol.TakeDamage(1);
        }
    }
}

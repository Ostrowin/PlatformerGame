using UnityEngine;
using System.Collections;

public class AttackHandler : MonoBehaviour
{
    public Transform attackPoint;
    public float strongAttackRange = 1f;
    public float weakAttackRange = 1.5f;
    public float attackForce = 7f;
    public LayerMask enemyLayers;
    
    public static bool isPerformingSpecialAttack = false;
    private Vector2 lastMoveDirection = Vector2.right; // Domyślnie patrzy w prawo
    
    private bool canBasicAttack = true;
    private bool canStrongAttack = true;
    private bool canWeakAttack = true;

    public float basicAttackCooldown = 0.5f;
    public float strongAttackCooldown = 2f;
    public float weakAttackCooldown = 1f;

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
        
        // 🔥 Atak podstawowy
        keyManager.RegisterCombination(new KeyCode[] { KeyCode.E }, () => BasicAttack());
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        if (move != 0)
        {
            lastMoveDirection = new Vector2(move, 0).normalized; // 🔥 Zapamiętujemy kierunek ruchu
        }
    }

    void StrongAttack(Vector2 direction)
    {
        if (isPerformingSpecialAttack || !canStrongAttack) return;

        isPerformingSpecialAttack = true;
        canStrongAttack = false;
        Debug.Log($"Silny atak w kierunku {direction}");
        PerformAttack(direction, strongAttackRange, attackForce, Color.yellow, 3);

        StartCoroutine(ResetSpecialAttack());
        StartCoroutine(ResetAttackCooldown(nameof(canStrongAttack), strongAttackCooldown));
    }

    void WeakAttack(Vector2 direction)
    {
        if (isPerformingSpecialAttack || !canWeakAttack) return;

        isPerformingSpecialAttack = true;
        canWeakAttack = false;
        Debug.Log($"Słabszy atak w kierunku {direction}");
        PerformAttack(direction, weakAttackRange, attackForce / 2, new Color(0.5f, 0f, 0.5f), 1);

        StartCoroutine(ResetSpecialAttack());
        StartCoroutine(ResetAttackCooldown(nameof(canWeakAttack), weakAttackCooldown));
    }

    void BasicAttack()
    {
        if (isPerformingSpecialAttack || !canBasicAttack) return;

        canBasicAttack = false; // 🔥 Blokujemy atak do końca cooldownu
        Debug.Log($"Podstawowy atak w kierunku {lastMoveDirection}");
        PerformAttack(lastMoveDirection, 1f, attackForce / 2, Color.white, 2);
        
        StartCoroutine(ResetAttackCooldown(nameof(canBasicAttack), basicAttackCooldown));
    }

    IEnumerator ResetSpecialAttack()
    {
        yield return new WaitForSeconds(0.3f); // 🔥 Czas, po którym `E` znowu działa
        isPerformingSpecialAttack = false;
    }

    IEnumerator ResetAttackCooldown(string attackType, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        if (attackType == nameof(canBasicAttack))
            canBasicAttack = true;
        else if (attackType == nameof(canStrongAttack))
            canStrongAttack = true;
        else if (attackType == nameof(canWeakAttack))
            canWeakAttack = true;

        Debug.Log($"{attackType} gotowy do użycia!");
    }

    void PerformAttack(Vector2 direction, float range, float force, Color attackColor, int damage)
    {
        Vector3 attackPosition = transform.position + (Vector3)(direction * range);

        // 🔥 Tymczasowa wizualizacja ataku
        GameObject attackEffect = GameObject.CreatePrimitive(PrimitiveType.Quad);
        attackEffect.transform.position = attackPosition;
        attackEffect.transform.position += new Vector3(0, 0, -1); // 🔥 Wypycha do przodu
        attackEffect.transform.localScale = new Vector3(range, range, 1);

        Renderer attackRenderer = attackEffect.GetComponent<Renderer>();
        if (attackRenderer != null)
        {
            attackRenderer.sortingLayerName = "Foreground";
            attackRenderer.sortingOrder = 10;
            attackRenderer.material.color = attackColor;
        }

        Destroy(attackEffect, 0.1f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, range, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"Trafiono: {enemy.name} | Zadano {damage} obrażeń!");

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

            if (enemyChase != null)
            {
                enemyChase.TakeDamage(damage);
            }
            else if (enemyPatrol != null)
            {
                enemyPatrol.TakeDamage(damage);
            }
        }
    }
}

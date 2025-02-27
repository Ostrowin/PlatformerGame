using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public int attackDamage = 1;
    public LayerMask playerLayer;

    private bool canAttack = true;

    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (!canAttack) return;

        Collider2D player = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (player != null)
        {
            StartCoroutine(AttackPlayer(player.gameObject));
        }
    }

    private IEnumerator AttackPlayer(GameObject player)
    {
        canAttack = false;
        Debug.Log($"{name} atakuje {player.name}!");

        // 🔥 Pobieramy PlayerHealth
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogError("❌ Gracz nie ma komponentu PlayerHealth!");
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    private void Start()
    {
        gameObject.AddComponent<EnemyHealth>();
        gameObject.AddComponent<EnemyPatrol>();
        gameObject.AddComponent<EnemyChase>();
        gameObject.AddComponent<EnemyAttack>();
    }
}

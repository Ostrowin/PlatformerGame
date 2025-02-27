using UnityEngine;

public class EnemyGhost : MonoBehaviour
{
    private void Start()
    {
        gameObject.AddComponent<EnemyHealth>();
        gameObject.AddComponent<EnemyPatrol>();
        gameObject.AddComponent<EnemyTouchDamage>();
    }
}

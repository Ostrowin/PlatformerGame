using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    private void Start()
    {
        gameObject.AddComponent<EnemyHealth>();
        gameObject.AddComponent<EnemyShoot>();
        gameObject.AddComponent<EnemyKeepDistance>();
    }
}

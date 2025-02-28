using UnityEngine;

public class PlayerUprightBalance : MonoBehaviour
{
    public float balanceSpeed = 3f; // Jak szybko gracz wraca do pionu

    void FixedUpdate()
    {
        MaintainUprightPosition();
    }

    private void MaintainUprightPosition()
    {
        float currentZRotation = transform.rotation.eulerAngles.z;

        if (currentZRotation > 5f && currentZRotation < 355f) 
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * balanceSpeed);
        }
    }
}

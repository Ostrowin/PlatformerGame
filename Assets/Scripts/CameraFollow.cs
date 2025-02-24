using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f; // Im wyższa wartość, tym szybsza reakcja
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Docelowa pozycja kamery (z uwzględnieniem offsetu)
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}

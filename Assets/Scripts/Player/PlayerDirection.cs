using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    public Vector2 lastMoveDirection { get; private set; } = Vector2.right;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) lastMoveDirection = Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow)) lastMoveDirection = Vector2.right;
    }
}

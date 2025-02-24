using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform player; // Gracz
    public float parallaxSpeed = 0.2f; // Jak wolno porusza się tło

    private float startX;
    private float length;

    void Start()
    {
        startX = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x; // Pobranie szerokości obrazka
    }

    void Update()
    {
        float move = player.position.x * parallaxSpeed;
        transform.position = new Vector3(startX + move, transform.position.y, transform.position.z);

        // 🔥 Zapętlanie tła (infinite scrolling)
        if (player.position.x > startX + length)
        {
            startX += length;
        }
        else if (player.position.x < startX - length)
        {
            startX -= length;
        }
    }
}

using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float fallSpeed = 2f;
    public int healAmount = 3;
    public float lifetime = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance?.PlayPickup();
            other.GetComponent<PlayerController>().Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
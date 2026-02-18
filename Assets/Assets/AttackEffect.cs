
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public float lifetime = 0.15f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>()?.TakeDamage(1);
        }
    }
}

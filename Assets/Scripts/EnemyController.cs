using UnityEngine;
// using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float speed = 2f;
    public int health = 1;
    public float attackRange = 0.8f;
    public int damage = 1;
    public float attackCooldown = 1f;
    float nextAttack;

    Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > attackRange)
        {
            // Move toward player
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (Time.time > nextAttack)
        {
            // Attack
            nextAttack = Time.time + attackCooldown;
            player.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            // Add memory on kill
            MemorySystem.Instance?.AddMemory(MemorySystem.Instance.memoryPerKill);
            Destroy(gameObject);
        }
    }
}
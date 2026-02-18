using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2f;
    public int health = 1;
    public float attackRange = 0.8f;
    public int damage = 8;
    public float attackCooldown = 1f;

    float nextAttack;

    Transform player;
    Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!player) return;

        PlayerController playerController = player.GetComponent<PlayerController>();
        int score = playerController != null ? playerController.score : 0;

        int scaledDamage = damage + (score / 100);

        float dist = Vector2.Distance(rb.position, player.position);

        if (dist > attackRange)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
        else if (Time.time > nextAttack)
        {
            nextAttack = Time.time + attackCooldown;
            playerController?.TakeDamage(scaledDamage);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            // Audio & GameFeel
            AudioManager.Instance?.PlayEnemyDeath();
            GameFeel.Instance?.OnEnemyDeath();

            PlayerController playerController = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
            playerController?.OnEnemyKilled();

            MemorySystem.Instance?.AddMemory(MemorySystem.Instance.memoryPerKill);

            Destroy(gameObject);
        }
    }
}
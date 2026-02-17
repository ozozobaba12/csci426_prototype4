using UnityEngine;
using UnityEngine.InputSystem;
// using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public int maxHealth = 20;
    public float attackRange = 1f;
    public int damage = 1;
    public float attackCooldown = 0.5f;
    float nextAttack;
    public int health;
    bool isDead;

    void Start()  => health = maxHealth;

    void Update()
    {
        // Movement (WASD)
        Vector2 move = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) move.y += 1;
        if (Keyboard.current.sKey.isPressed) move.y -= 1;
        if (Keyboard.current.aKey.isPressed) move.x -= 1;
        if (Keyboard.current.dKey.isPressed) move.x += 1;
        transform.Translate(move.normalized * speed * Time.deltaTime);

        // Attack
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time > nextAttack)
        {
            nextAttack = Time.time + attackCooldown;
            Attack();
        }
    }

    void Attack()
    {

        AudioManager.Instance?.PlayAttack();

        // Notify memory system (may cost blood if above 80%)
        MemorySystem.Instance?.OnPlayerAttack(this);

        // Find enemies in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<EnemyController>().TakeDamage(damage);
                GameFeel.Instance?.OnPlayerHit();
                break; // Hit one enemy per attack
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        AudioManager.Instance?.PlayPlayerHurt();
        GameFeel.Instance?.OnPlayerHurt();

        health -= dmg;
        if (health <= 0)
        {
            health = 0;  // Clamp to 0 for UI
            isDead = true;
            GameManager.Instance.PlayerDied();
            // Don't destroy immediately — let UI update first
        }
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
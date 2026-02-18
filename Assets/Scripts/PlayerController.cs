using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public int maxHealth = 20;
    public float attackRange = 1f;
    public int damage = 1;
    public float attackCooldown = 0.5f;
    public GameObject attackEffectPrefab;
    public float attackDistance = 1f;
    public float scoreMultiplier = 1f;
    public float multiplierIncrease = 0.1f;
    public float maxMultiplier = 5f;

    bool tookDamageSinceLastKill = false;
    public int score = 0;

    public float invincibleDuration = 5f;
    public float boostedSpeed = 8f;

    bool isInvincible = false;
    float invincibleTimer = 0f;
    float normalSpeed;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    float nextAttack;
    public int health;
    bool isDead;

    Animator anim;
    Vector2 lastMove = Vector2.down;

    void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        normalSpeed = speed;
        UpdateUI();
    }

    void Update()
    {
        if (isDead) return;

        Vector2 move = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) move.y += 1;
        if (Keyboard.current.sKey.isPressed) move.y -= 1;
        if (Keyboard.current.aKey.isPressed) move.x -= 1;
        if (Keyboard.current.dKey.isPressed) move.x += 1;

        transform.Translate(move.normalized * speed * Time.deltaTime);

        if (move != Vector2.zero)
        {
            lastMove = move;
            if (anim) anim.SetBool("isMoving", true);
        }
        else
        {
            if (anim) anim.SetBool("isMoving", false);
        }

        if (anim)
        {
            anim.SetFloat("moveX", lastMove.x);
            anim.SetFloat("moveY", lastMove.y);
        }

        // Attack
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time > nextAttack)
        {
            nextAttack = Time.time + attackCooldown;
            if (anim && HasAnimatorParameter(anim, "Attack")) 
                anim.SetTrigger("Attack");
            Attack();
        }

        // Invincibility check
        if (!isInvincible && MemorySystem.Instance != null &&
            MemorySystem.Instance.memory >= MemorySystem.Instance.maxMemory)
        {
            ActivateInvincibility();
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                speed = normalSpeed;
            }
        }
    }

    void Attack()
    {
        // Audio
        AudioManager.Instance?.PlayAttack();
        
        // Memory system (may cost blood if above 80%)
        MemorySystem.Instance?.OnPlayerAttack(this);

        // Spawn attack effect
        Vector2 attackDir = lastMove.normalized;
        Vector3 spawnPos = transform.position + (Vector3)(attackDir * attackDistance);
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        Instantiate(attackEffectPrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        // Find enemies in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<EnemyController>().TakeDamage(damage);
                GameFeel.Instance?.OnPlayerHit();
                break;
            }
        }
    }

    void ActivateInvincibility()
    {
        isInvincible = true;
        invincibleTimer = invincibleDuration;
        speed = boostedSpeed;
        MemorySystem.Instance?.ResetMemory();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (multiplierText != null)
            multiplierText.text = "x" + scoreMultiplier.ToString("F1");
    }

    public void OnEnemyKilled()
    {
        if (!tookDamageSinceLastKill)
        {
            scoreMultiplier += multiplierIncrease;
            scoreMultiplier = Mathf.Min(scoreMultiplier, maxMultiplier);
        }

        tookDamageSinceLastKill = false;
        score += Mathf.RoundToInt(10 * scoreMultiplier);
        UpdateUI();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        // Audio & GameFeel
        AudioManager.Instance?.PlayPlayerHurt();
        GameFeel.Instance?.OnPlayerHurt();

        if (!isInvincible)
        {
            health -= dmg;
            tookDamageSinceLastKill = true;
            scoreMultiplier = 1f;
        }

        if (health <= 0)
        {
            health = 0;
            isDead = true;
            GameManager.Instance.PlayerDied();
        }

        UpdateUI();
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 attackDir = lastMove.normalized;
        Vector2 attackPos = (Vector2)transform.position + attackDir * attackRange;
        Gizmos.DrawWireSphere(attackPos, 0.5f);
    }

    bool HasAnimatorParameter(Animator animator, string paramName)
{
    foreach (AnimatorControllerParameter param in animator.parameters)
    {
        if (param.name == paramName) return true;
    }
    return false;
}
}
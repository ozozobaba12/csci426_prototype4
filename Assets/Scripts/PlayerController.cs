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
        Vector2 move = Vector2.zero;

if (Keyboard.current.wKey.isPressed) move.y += 1;
if (Keyboard.current.sKey.isPressed) move.y -= 1;
if (Keyboard.current.aKey.isPressed) move.x -= 1;
if (Keyboard.current.dKey.isPressed) move.x += 1;


transform.Translate(move.normalized * speed * Time.deltaTime);

// ---- FIX STARTS HERE ----

if (move != Vector2.zero)
{
    lastMove = move;
    anim.SetBool("isMoving", true);
}
else
{
    anim.SetBool("isMoving", false);
}

// Always send last direction to animator
anim.SetFloat("moveX", lastMove.x);
anim.SetFloat("moveY", lastMove.y);


        // ----- ATTACK -----

        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time > nextAttack)
        {
            nextAttack = Time.time + attackCooldown;
            anim.SetTrigger("Attack"); // optional if you add attack animation
            Attack();
        }

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

    void ActivateInvincibility()
{
    isInvincible = true;
    invincibleTimer = invincibleDuration;
    speed = boostedSpeed;

    MemorySystem.Instance.ResetMemory(); // You must have this function
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

    Debug.Log("Score: " + score + " Multiplier: " + scoreMultiplier);
    UpdateUI();
}



    
      void Attack()
{
    Vector2 attackDir = lastMove.normalized;

    Vector3 spawnPos = transform.position + (Vector3)(attackDir * attackDistance);

    float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

    Instantiate(attackEffectPrefab, spawnPos, Quaternion.Euler(0, 0, angle));
}


    

   public void TakeDamage(int dmg)
{
    if (isDead) return;

    if (!isInvincible)
    {
        health -= dmg;
        tookDamageSinceLastKill = true;
        scoreMultiplier = 1f;   // reset combo
    }

    if (health <= 0)
    {
        health = 0;
        isDead = true;
        GameManager.Instance.PlayerDied();
    }

    scoreMultiplier = 1f;
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
    Vector2 boxSize = new Vector2(0.8f, 0.8f);

    Gizmos.DrawWireCube(attackPos, boxSize);
}

}

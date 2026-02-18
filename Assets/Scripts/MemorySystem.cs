using UnityEngine;
using UnityEngine.UI;

public class MemorySystem : MonoBehaviour
{
    public static MemorySystem Instance;
    
    public Image fillImage;
    public float maxMemory = 100f;
    public float memory;
    
    [Header("Thresholds")]
    public float threshold1 = 40f;
    public float threshold2 = 80f;
    
    [Header("Decay")]
    public float decayRate = 2f;
    
    [Header("Self-Damage")]
    public int selfDamagePerAttack = 2;
    
    [Header("Kill Reward")]
    public float memoryPerKill = 10f;

    bool hasWon;

    public float MemoryPercent => memory / maxMemory * 100f;
    public bool AboveThreshold1 => MemoryPercent >= threshold1;
    public bool AboveThreshold2 => MemoryPercent >= threshold2;

    void Awake() => Instance = this;

    void Start()
    {
        memory = 0f;
        UpdateUI();
    }

    void Update()
    {
        if (hasWon) return;

        if (AboveThreshold1)
        {
            memory -= decayRate * Time.deltaTime;
            memory = Mathf.Max(0, memory);
        }
        
        UpdateUI();
    }

    void UpdateUI()
    {
        if (fillImage)
            fillImage.fillAmount = memory / maxMemory;
    }

    public void AddMemory(float amount)
    {
        if (hasWon) return;

        memory = Mathf.Min(memory + amount, maxMemory);

        // Win condition — uncomment if you want memory 100% to trigger win
        if (memory >= maxMemory)
        {
            HintSystem.Instance?.QueueHint("You are becoming invicible! Resetting...", Color.cyan, true);
        //     hasWon = true;
        //     GameManager.Instance.PlayerWon();
        }
    }

    public void OnPlayerAttack(PlayerController player)
    {
        if (AboveThreshold2)
        {
            player.TakeDamage(selfDamagePerAttack);
        }
    }

    public void ResetMemory()
    {
        memory = 0f;
        hasWon = false;
        UpdateUI();
    }
}
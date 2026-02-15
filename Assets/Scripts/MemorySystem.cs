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
    public float decayRate = 2f;  // Memory loss per second above 40%
    
    [Header("Self-Damage")]
    public int selfDamagePerAttack = 2;  // Blood cost per attack above 80%
    
    [Header("Kill Reward")]
    public float memoryPerKill = 10f;

    bool hasWon;

    public float MemoryPercent => memory / maxMemory * 100f;
    public bool AboveThreshold1 => MemoryPercent >= threshold1;
    public bool AboveThreshold2 => MemoryPercent >= threshold2;

    void Awake() => Instance = this;

    void Start()
    {
        memory = 0f;  // Start empty
        UpdateUI();
    }

    void Update()
    {
        if (hasWon) return;

        // Decay above 40%
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

        // Win condition
        if (memory >= maxMemory)
        {
            hasWon = true;
            GameManager.Instance.PlayerWon();
        }
    }

    public void OnPlayerAttack(PlayerController player)
    {
        // Above 80%: attacks cost blood
        if (AboveThreshold2)
        {
            player.TakeDamage(selfDamagePerAttack);
        }
    }
}
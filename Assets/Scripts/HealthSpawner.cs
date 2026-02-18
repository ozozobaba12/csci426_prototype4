using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    public GameObject healthPickupPrefab;
    public float checkInterval = 2f;
    public float spawnChance = 0.9f;  // 30% chance per check when below threshold
    public float healthThreshold = 50f;  // Spawn when below 50%
    public float xRange = 7f;
    public float spawnY = 6f;

    float nextCheck;
    PlayerController player;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {
        if (!player || Time.time < nextCheck) return;

        nextCheck = Time.time + checkInterval;

        float healthPercent = (float)player.health / player.maxHealth * 100f;
        
        if (healthPercent < healthThreshold && Random.value < spawnChance)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        float x = Random.Range(-xRange, xRange);
        Instantiate(healthPickupPrefab, new Vector3(x, spawnY, 0), Quaternion.identity);
    }
}
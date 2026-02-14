using UnityEngine;
// using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float baseSpawnRate = 2f;
    public float yRange = 4f;

    [Header("Memory Scaling")]
    public float minSpawnInterval = 0.5f;
    public float spawnRateMultiplier = 0.02f;  // How much memory affects spawn rate

    float nextSpawn;

    // void Start() => InvokeRepeating(nameof(Spawn), 1f, spawnRate);

    void Update()
    {
        if (Time.time > nextSpawn)
        {
            Spawn();
            
            // Faster spawns with higher memory
            float memoryBonus = 0f;
            if (MemorySystem.Instance)
                memoryBonus = MemorySystem.Instance.MemoryPercent * spawnRateMultiplier;
            
            float interval = Mathf.Max(minSpawnInterval, baseSpawnRate - memoryBonus);
            nextSpawn = Time.time + interval;
        }
    }

    void Spawn()
    {
        float y = Random.Range(-yRange, yRange);
        Instantiate(enemyPrefab, new Vector3(transform.position.x, y, 0), Quaternion.identity);
    }
}
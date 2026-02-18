using UnityEngine;
// using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Sprite[] enemySprites;
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
    Camera cam = Camera.main;

    float camHeight = cam.orthographicSize;
    float camWidth = camHeight * cam.aspect;

    Vector3 spawnPos = Vector3.zero;

    int side = Random.Range(0, 4);

    switch (side)
    {
        case 0:
            spawnPos = new Vector3(Random.Range(-camWidth, camWidth), camHeight + 1f, 0);
            break;

        case 1:
            spawnPos = new Vector3(Random.Range(-camWidth, camWidth), -camHeight - 1f, 0);
            break;

        case 2:
            spawnPos = new Vector3(-camWidth - 1f, Random.Range(-camHeight, camHeight), 0);
            break;

        case 3:
            spawnPos = new Vector3(camWidth + 1f, Random.Range(-camHeight, camHeight), 0);
            break;
    }

    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

    // 🔥 Assign random sprite
    if (enemySprites.Length > 0)
    {
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        sr.sprite = enemySprites[Random.Range(0, enemySprites.Length)];
    }
}



}
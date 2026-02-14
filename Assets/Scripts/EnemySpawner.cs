using UnityEngine;
// using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public float yRange = 4f;

    void Start() => InvokeRepeating(nameof(Spawn), 1f, spawnRate);

    void Spawn()
    {
        float y = Random.Range(-yRange, yRange);
        Instantiate(enemyPrefab, new Vector3(transform.position.x, y, 0), Quaternion.identity);
    }
}
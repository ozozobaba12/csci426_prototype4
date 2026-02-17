using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public static VFXSpawner Instance;

    [Header("Prefabs")]
    public GameObject hitVFX;
    public GameObject deathVFX;
    public GameObject healVFX;
    public GameObject playerHurtVFX;

    [Header("Settings")]
    public float vfxLifetime = 0.5f;

    void Awake() => Instance = this;

    public void SpawnHit(Vector3 position)
    {
        if (hitVFX)
            Destroy(Instantiate(hitVFX, position, Quaternion.identity), vfxLifetime);
    }

    public void SpawnDeath(Vector3 position)
    {
        if (deathVFX)
            Destroy(Instantiate(deathVFX, position, Quaternion.identity), vfxLifetime);
    }

    public void SpawnHeal(Vector3 position)
    {
        if (healVFX)
            Destroy(Instantiate(healVFX, position, Quaternion.identity), vfxLifetime);
    }

    public void SpawnPlayerHurt(Vector3 position)
    {
        if (playerHurtVFX)
            Destroy(Instantiate(playerHurtVFX, position, Quaternion.identity), vfxLifetime);
    }
}
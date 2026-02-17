using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Sound Effects")]
    public AudioClip playerAttack;
    public AudioClip enemyDeath;
    public AudioClip playerHurt;
    public AudioClip pickupCollected;
    public AudioClip warningThreshold1;
    public AudioClip warningThreshold2;
    public AudioClip lowHealthWarning;
    public AudioClip winSound;
    public AudioClip loseSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    void Awake()
    {
        Instance = this;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // Convenience methods
    public void PlayAttack() => PlaySound(playerAttack);
    public void PlayEnemyDeath() => PlaySound(enemyDeath);
    public void PlayPlayerHurt() => PlaySound(playerHurt);
    public void PlayPickup() => PlaySound(pickupCollected);
    public void PlayWarning1() => PlaySound(warningThreshold1);
    public void PlayWarning2() => PlaySound(warningThreshold2);
    public void PlayLowHealthWarning() => PlaySound(lowHealthWarning);
    public void PlayWin() => PlaySound(winSound);
    public void PlayLose() => PlaySound(loseSound);
}
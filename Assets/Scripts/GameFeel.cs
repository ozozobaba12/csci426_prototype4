using UnityEngine;
using System.Collections;

public class GameFeel : MonoBehaviour
{
    public static GameFeel Instance;

    [Header("References")]
    public Camera mainCamera;

    [Header("Hitstop")]
    public float hitStopDuration = 0.05f;
    public float strongHitStopDuration = 0.1f;

    [Header("Screen Shake")]
    public float shakeDuration = 0.1f;
    public float shakeIntensity = 0.2f;
    public float strongShakeIntensity = 0.4f;

    Vector3 originalCamPos;
    bool isShaking;
    bool isHitStopping;

    void Awake()
    {
        Instance = this;
        if (!mainCamera) mainCamera = Camera.main;
        originalCamPos = mainCamera.transform.position;
    }

    // Light hit — player attacks enemy
    public void OnPlayerHit()
    {
        if (!isHitStopping)
            StartCoroutine(DoHitStop(hitStopDuration));
        StartCoroutine(DoShake(shakeDuration, shakeIntensity));
    }

    // Heavy hit — player takes damage
    public void OnPlayerHurt()
    {
        if (!isHitStopping)
            StartCoroutine(DoHitStop(strongHitStopDuration));
        StartCoroutine(DoShake(shakeDuration, strongShakeIntensity));
    }

    // Enemy death — satisfying feedback
    public void OnEnemyDeath()
    {
        if (!isHitStopping)
            StartCoroutine(DoHitStop(hitStopDuration));
        StartCoroutine(DoShake(shakeDuration * 0.5f, shakeIntensity * 0.5f));
    }

    // End — big impact
    public void OnEnd()
    {
        if (!isHitStopping)
            StartCoroutine(DoHitStop(0.2f));
        StartCoroutine(DoShake(0.3f, strongShakeIntensity));
    }

    IEnumerator DoHitStop(float duration)
    {
        isHitStopping = true;
        // float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        isHitStopping = false;
    }

    IEnumerator DoShake(float duration, float intensity)
    {
        if (isShaking) yield break;
        isShaking = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            mainCamera.transform.position = originalCamPos + new Vector3(x, y, 0);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalCamPos;
        isShaking = false;
    }

    public void ResetTimeScale()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
        isHitStopping = false;
        isShaking = false;
        if (mainCamera)
            mainCamera.transform.position = originalCamPos;
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
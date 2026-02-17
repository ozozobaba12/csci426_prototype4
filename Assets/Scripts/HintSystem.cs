using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HintSystem : MonoBehaviour
{
    public static HintSystem Instance;

    [Header("UI References")]
    public TextMeshProUGUI hintText;
    public Image screenFlash;
    public CanvasGroup hintPanel;

    [Header("Settings")]
    public float hintDuration = 1f;
    public float fadeSpeed = 0.5f;

    bool threshold1Triggered;
    bool threshold2Triggered;
    bool lowHealthTriggered;

    void Awake() => Instance = this;

    void Start()
    {
        StartCoroutine(ShowTutorial());
    }

    IEnumerator ShowTutorial()
    {
        yield return ShowHint("Enemies approaching from the right! Be careful to be attacked", Color.white);
        // yield return new WaitForSeconds(0.3f);
        // yield return ShowHint("Use WASD to move, SPACE to attack when close", Color.white);
        // yield return new WaitForSeconds(1f);
        yield return ShowHint("Kill enemies to fill your MEMORY bar to win.", Color.cyan);
    }

    void Update()
    {
        CheckThresholds();
    }

    void CheckThresholds()
    {
        if (!MemorySystem.Instance) return;

        float memPct = MemorySystem.Instance.MemoryPercent;

        // Memory 40% threshold
        if (!threshold1Triggered && memPct >= 40f)
        {
            threshold1Triggered = true;
            StartCoroutine(ShowHint("Memory unstable! It will slowly decay now.", Color.yellow));
            FlashScreen(Color.yellow);
        }

        // Memory 80% threshold
        if (!threshold2Triggered && memPct >= 80f)
        {
            threshold2Triggered = true;
            StartCoroutine(ShowHint("DANGER! Attacks now cost blood.", Color.red));
            FlashScreen(Color.red);
        }

        // Low health warning
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player)
        {
            float healthPct = (float)player.health / player.maxHealth * 100f;
            if (!lowHealthTriggered && healthPct < 50f)
            {
                lowHealthTriggered = true;
                StartCoroutine(ShowHint("Blood low! Collect red pickups falling from above.", Color.green));
            }
            if (healthPct >= 50f) lowHealthTriggered = false;  // Reset for re-trigger
        }
    }

    public IEnumerator ShowHint(string message, Color color)
    {
        hintText.text = message;
        hintText.color = color;
        hintPanel.alpha = 1f;

        yield return new WaitForSeconds(hintDuration);

        while (hintPanel.alpha > 0)
        {
            hintPanel.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public void FlashScreen(Color color)
    {
        StartCoroutine(DoFlash(color));
    }

    IEnumerator DoFlash(Color color)
    {
        color.a = 0.3f;
        screenFlash.color = color;
        screenFlash.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        float alpha = 0.3f;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * 2f;
            color.a = alpha;
            screenFlash.color = color;
            yield return null;
        }

        screenFlash.gameObject.SetActive(false);
    }
}
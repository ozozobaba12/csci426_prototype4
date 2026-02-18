using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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
    public float fontSize = 50f;

    [Header("State")]
    bool threshold1Triggered;
    bool threshold2Triggered;
    bool lowHealthTriggered;
    bool gameEnded;

    Queue<HintData> hintQueue = new Queue<HintData>();
    bool isShowingHint;

    struct HintData
    {
        public string message;
        public Color color;
        public bool flash;
    }

    void Awake() => Instance = this;

    void Start()
    {
        // AudioManager.Instance?.PlayGameStart();

        hintText.fontSize = fontSize;
        QueueHint("Enemies approaching! Kill enemies to fill GreenBar.", Color.white);
    }

    public void QueueHint(string message, Color color, bool flash = false)
    {
        hintQueue.Enqueue(new HintData { message = message, color = color, flash = flash });
    }

    void Update()
    {
        if (gameEnded) return;

        CheckThresholds();
        ProcessQueue();
    }

    void CheckThresholds()
    {
        if (!MemorySystem.Instance) return;

        float memPct = MemorySystem.Instance.MemoryPercent;

        // Memory 40% threshold
        if (!threshold1Triggered && memPct >= 40f)
        {
            threshold1Triggered = true;
            AudioManager.Instance?.PlayWarning1();
            QueueHint("Unstable! GreenBar slowly decaying now", Color.yellow, true);
        }

        // Memory 80% threshold
        if (!threshold2Triggered && memPct >= 90f)
        {
            threshold2Triggered = true;
            AudioManager.Instance?.PlayWarning2();
            QueueHint("DANGER! Attacks now cost blood", Color.red, true);
        }

        // Low health warning
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player)
        {
            float healthPct = (float)player.health / player.maxHealth * 100f;
            if (!lowHealthTriggered && healthPct < 70f)
            {
                lowHealthTriggered = true;
                AudioManager.Instance?.PlayLowHealthWarning();
                QueueHint("Blood low! Collect red pickups", Color.green, false);
            }
            if (healthPct >= 71f) lowHealthTriggered = false;  // Reset for re-trigger
        }
    }

    void ProcessQueue()
    {
        if (!isShowingHint && hintQueue.Count > 0)
        {
            var hint = hintQueue.Dequeue();
            StartCoroutine(ShowHint(hint));
        }
    }

    IEnumerator ShowHint(HintData hint)
    {
        isShowingHint = true;

        hintText.text = hint.message;
        hintText.color = hint.color;
        hintPanel.alpha = 1f;
        if (hint.flash)
            FlashScreen(hint.color);

        yield return new WaitForSeconds(hintDuration);

        while (hintPanel.alpha > 0 && !gameEnded)
        {
            hintPanel.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }

        hintPanel.alpha = 0f;
        isShowingHint = false;
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

    public void OnGameEnd()
    {
        gameEnded = true;
        StopAllCoroutines();
        hintQueue.Clear();
        hintPanel.alpha = 0f;
        screenFlash.gameObject.SetActive(false);
    }
}
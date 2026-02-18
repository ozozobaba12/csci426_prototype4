using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject gameOverPanel;
    public GameObject winUI;
    public float deathDelay = 0.5f;
    public TextMeshProUGUI finalScoreText;

    void Awake() => Instance = this;

    public void PlayerDied()
    {
        AudioManager.Instance?.PlayLose();
        GameFeel.Instance?.OnEnd();
        HintSystem.Instance?.OnGameEnd();
        Invoke(nameof(ShowGameOver), deathDelay);
    }

    void ShowGameOver()
    {
        int score = 0;
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player) score = player.score;

        ClearGameObjects();
        gameOverPanel.SetActive(true);

        if (finalScoreText)
        finalScoreText.text = "Score: " + score;
        
        LeaderboardManager.Instance?.SetFinalScore(score);       
        LeaderboardManager.Instance?.ShowLeaderboard();
        Time.timeScale = 0f;
    }

    public void PlayerWon()
    {
        AudioManager.Instance?.PlayWin();
        GameFeel.Instance?.OnEnd();
        HintSystem.Instance?.OnGameEnd();
        Invoke(nameof(ShowWin), 0.3f);
    }

    void ShowWin()
    {
        ClearGameObjects();
        winUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void ClearGameObjects()
    {
        // Hide player
        GameObject player = GameObject.FindWithTag("Player");
        if (player) player.SetActive(false);

        // Destroy all enemies
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        // Destroy all health pickups
        foreach (HealthPickup pickup in FindObjectsByType<HealthPickup>(FindObjectsSortMode.None))
        {
            Destroy(pickup.gameObject);
        }
    }

    public void Restart()
    {
        GameFeel.Instance?.ResetTimeScale();
        Time.timeScale = 1f;
        LeaderboardManager.Instance?.ResetSubmission();
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
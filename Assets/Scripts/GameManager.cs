using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
public GameObject gameOverPanel;

    public GameObject winUI;
    public float deathDelay = 0.5f;

    void Awake() => Instance = this;

    public void PlayerDied()
    {
        Invoke(nameof(ShowGameOver), deathDelay);
    }

public TextMeshProUGUI finalScoreText;

void ShowGameOver()
{
    gameOverPanel.SetActive(true);
    finalScoreText.text = "Score: " + FindObjectOfType<PlayerController>().score;
    LeaderboardManager.Instance.ShowLeaderboard();
    Time.timeScale = 0f;
}




    public void PlayerWon()
    {
        Invoke(nameof(ShowWin), 0.3f);
    }

    void ShowWin()
    {
        winUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        // Debug.Log("Restart clicked!");  // Check Console
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );

        LeaderboardManager.Instance.ResetSubmission();

    }
}
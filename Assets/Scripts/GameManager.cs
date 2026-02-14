using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject gameOverUI;
    public float deathDelay = 0.5f;

    void Awake() => Instance = this;

    public void PlayerDied()
    {
        Invoke(nameof(ShowGameOver), deathDelay);
    }

    void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    public void Restart()
    {
        Debug.Log("Restart clicked!");  // Check Console
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
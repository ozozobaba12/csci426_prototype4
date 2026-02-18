using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    const int maxEntries = 10;

    [Header("UI References")]
    public TMP_InputField nameInputField;
    public TextMeshProUGUI leaderboardText;
    bool hasSubmittedThisGame=false;

    void Awake()
    {
        Instance = this;
        LoadLeaderboard();
        DisplayLeaderboard();
    }

    public void AddEntry(string playerName, int score)
    {
        entries.Add(new LeaderboardEntry(playerName, score));

        // Sort descending
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        // Keep only top 10
        if (entries.Count > maxEntries)
            entries.RemoveAt(entries.Count - 1);

        SaveLeaderboard();
        DisplayLeaderboard();
    }

    public void ShowLeaderboard()
{
    DisplayLeaderboard();
}


    public void SubmitScore()
{
    if (hasSubmittedThisGame)
        return;

    string name = nameInputField.text.ToUpper();

    if (name.Length != 3)
        return;

    int finalScore = FindObjectOfType<PlayerController>().score;

    AddEntry(name, finalScore);

    hasSubmittedThisGame = true;

    nameInputField.interactable = false;  // lock input
}


    void DisplayLeaderboard()
    {
        if (!leaderboardText) return;

        leaderboardText.text = "";

        for (int i = 0; i < entries.Count; i++)
        {
            leaderboardText.text +=
                $"{i + 1}. {entries[i].playerName}  {entries[i].score}\n";
        }
    }

    void SaveLeaderboard()
    {
        PlayerPrefs.SetInt("LeaderboardCount", entries.Count);

        for (int i = 0; i < entries.Count; i++)
        {
            PlayerPrefs.SetString("Name_" + i, entries[i].playerName);
            PlayerPrefs.SetInt("Score_" + i, entries[i].score);
        }

        PlayerPrefs.Save();
    }

    void LoadLeaderboard()
    {
        entries.Clear();

        int count = PlayerPrefs.GetInt("LeaderboardCount", 0);

        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString("Name_" + i);
            int score = PlayerPrefs.GetInt("Score_" + i);

            entries.Add(new LeaderboardEntry(name, score));
        }
    }

    public void ResetSubmission()
{
    hasSubmittedThisGame = false;

    if (nameInputField != null)
        nameInputField.interactable = true;
}

}

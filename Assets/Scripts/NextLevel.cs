using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    
    public GameObject NextLevelPanel;
    public UFOController player;
    public float scoreToNextLevel = 500f;
    void start()
    {
        scoreToNextLevel = 500f;
        if (NextLevelPanel != null)
            NextLevelPanel.SetActive(false);
    }

    void Update()
    {
        if (player != null && player.score >= scoreToNextLevel)
        {
            LoadNextLevel();
            scoreToNextLevel = scoreToNextLevel + 200f;
        }
    }
    public void LoadNextLevel()
    {
        if (NextLevelPanel != null)
        {
            NextLevelPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    
}

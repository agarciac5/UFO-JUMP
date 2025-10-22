using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public UFOController player;
    public float scoreToNextLevel = 500f;
    void start()
    {
       scoreToNextLevel = 500f;
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

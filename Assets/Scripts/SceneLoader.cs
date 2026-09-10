using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelScene");
    }

    public void LoadGame(int level)
    {
        StageManager.Instance.SetLevel(level);
        SceneManager.LoadScene("GameScene");
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene("GameScene");
    }
}